using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public class WorldDataService
    {
        public virtual MudObject GetObject(String Path) { throw new NotImplementedException(); }
        public virtual MudObject ReloadObject(String Path) { throw new NotImplementedException(); }

        public virtual void PersistInstance(MudObject Object) { throw new NotImplementedException(); }
        public virtual void ForgetInstance(MudObject Object) { throw new NotImplementedException(); }

        public virtual Tuple<bool, String> LoadSourceFile(String Path) { throw new NotImplementedException(); }

        public virtual int Save() { throw new NotImplementedException(); }

        public virtual void Initialize() { throw new NotImplementedException(); }

        protected Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();
        protected Dictionary<String, MudObject> ActiveInstances = new Dictionary<String, MudObject>();
        
        protected static void SplitObjectName(String FullName, out String BasePath, out String InstanceName)
        {
            var split = FullName.IndexOf('@');
            if (split > 0)
            {
                BasePath = FullName.Substring(0, split);

                if (split < FullName.Length - 1)
                    InstanceName = FullName.Substring(split + 1);
                else
                    InstanceName = null;
            }
            else
            {
                BasePath = FullName;
                InstanceName = null;
            }
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
            if (baseObject == null)
            {
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
                Core.GlobalRules.ConsiderPerformRule("update", newMudObject);
                return newMudObject;
            }
            else
            {
                throw new InvalidProgramException();
            }
        }

        public RMUD.MudObject ResetObject(string Path)
        {
            Path = Path.Replace('\\', '/');

            if (NamedObjects.ContainsKey(Path))
            {
                var existing = NamedObjects[Path];
                existing.State = ObjectState.Destroyed;

                var newObject = Activator.CreateInstance(existing.GetType()) as MudObject;
                NamedObjects.Upsert(Path, newObject);
                MudObject.InitializeObject(newObject);

                // Any object marked with the 'preserve?' flag should be kept. Other objects should be discarded.
                foreach (var item in existing.EnumerateObjectsAndRelloc())
                    if (item.Item1.GetPropertyOrDefault<bool>("preserve?"))
                    {
                        newObject.Add(item.Item1, item.Item2);
                        item.Item1.Location = newObject;
                    }

                if (existing.Location != null)
                {
                    var loc = existing.Location.RelativeLocationOf(existing);
                    MudObject.Move(newObject, existing.Location, loc);
                    MudObject.Move(existing, null, RelativeLocations.None);
                }

                existing.Destroy(false);

                return newObject;
            }
            else
                return null;
        }

    }
}
