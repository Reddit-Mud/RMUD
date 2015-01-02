using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    public partial class GithubDatabase
    {
        private Dictionary<String, MudObject> ActiveInstances = new Dictionary<String, MudObject>();
        
        public void PersistInstance(MudObject Object)
        {
            if (Object.IsPersistent) return; //The object is already persistent.
            if (ActiveInstances.ContainsKey(Object.GetFullName()))
                throw new InvalidOperationException("An instance with this name is already persisted.");
            if (Object.IsNamedObject)
            {
                Object.IsPersistent = true;
                ActiveInstances.Upsert(Object.GetFullName(), Object);
                ReadPersistentObject(Object);
            }
            else
                throw new InvalidOperationException("Anonymous objects cannot be persisted.");
        }

        public void ForgetInstance(MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.IsPersistent = false;
        }

        public MudObject CreateInstance(String FullName)
        {
            FullName = FullName.Replace('\\', '/');

            String BasePath, InstanceName;
            SplitObjectName(FullName, out BasePath, out InstanceName);

            if (String.IsNullOrEmpty(InstanceName)) 
                throw new InvalidOperationException("Instance can't be empty.");
            if (String.IsNullOrEmpty(BasePath))
                throw new InvalidOperationException("Basepath can't be empty.");
                                    
            var baseObject = GetObject(BasePath);

            //We can't make an instance of nothing; this means that the base object has an error of some kind.
            if (baseObject == null) {
                Core.LogError("ERROR: Invalid baseObject: " + BasePath);
                return null;
            }

            //Create the new instance of the same class as the base type.
            var newMudObject = Activator.CreateInstance(baseObject.GetType()) as MudObject;

            //It should not be possible for newMudObject to be null.
            if (newMudObject != null)
            {
                newMudObject.Path = BasePath;
                newMudObject.Instance = InstanceName;

                newMudObject.Initialize(); //Initialize must call 'PersistInstance' to setup persistence.
                newMudObject.State = ObjectState.Alive;
                newMudObject.HandleMarkedUpdate();
                return newMudObject;
            }
            else
            {
                throw new InvalidProgramException();
            }
        }

        public int Save()
        {
            var counter = 0;
            foreach (var instance in ActiveInstances)
            {
                ++counter;
                SavePersistentObject(instance.Value);
            }
            return counter;
        }

        private void SavePersistentObject(MudObject Object)
        {
            var filename = DynamicPath + Object.GetFullName() + ".txt";
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            var data = Core.SerializeObject(Object);
            System.IO.File.WriteAllText(filename, data);
        }

        private void ReadPersistentObject(MudObject Object)
        {
            var filename = DynamicPath + Object.GetFullName() + ".txt";
            if (!System.IO.File.Exists(filename)) return;
            var data = System.IO.File.ReadAllText(filename);
            Core.DeserializeObject(Object, data);
        }
    }

    public partial class MudObject
    {
        public static void PersistInstance(MudObject Object) { Core.Database.PersistInstance(Object); }
        public static void ForgetInstance(MudObject Object) { Core.Database.ForgetInstance(Object); }
    }
}