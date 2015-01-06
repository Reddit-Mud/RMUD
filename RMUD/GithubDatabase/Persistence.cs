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
        override public void PersistInstance(MudObject Object)
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

        override public void ForgetInstance(MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.IsPersistent = false;
        }

        override public int Save()
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