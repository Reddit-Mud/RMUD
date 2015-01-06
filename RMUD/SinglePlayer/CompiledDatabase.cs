using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace RMUD.SinglePlayer
{
    public class CompiledDatabase : RMUD.WorldDataService
    {
        System.Reflection.Assembly SourceAssembly;
        String BaseObjectName;

        public CompiledDatabase(System.Reflection.Assembly SourceAssembly, String BaseObjectName)
        {
            this.SourceAssembly = SourceAssembly;
            this.BaseObjectName = BaseObjectName;
        }

        override public RMUD.MudObject GetObject(string Path)
        {
            Path = Path.Replace('\\', '/');

            String BasePath, InstanceName;
            SplitObjectName(Path, out BasePath, out InstanceName);

            if (!String.IsNullOrEmpty(InstanceName))
            {
                MudObject activeInstance = null;
                if (ActiveInstances.TryGetValue(Path, out activeInstance))
                    return activeInstance;
                else
                    return CreateInstance(Path);
            }
            else
            {
                MudObject r = null;

                if (NamedObjects.ContainsKey(BasePath))
                    r = NamedObjects[BasePath];
                else
                {
                    var typeName = BaseObjectName + "." + Path.Replace("/", ".");
                    var type = SourceAssembly.GetType(typeName);
                    if (type == null) return null;
                    r = Activator.CreateInstance(type) as MudObject;
                    if (r != null)
                    {
                        r.Path = Path;
                        r.State = ObjectState.Unitialized;
                        NamedObjects.Upsert(BasePath, r);
                    }
                }

                if (r != null && r.State == ObjectState.Unitialized)
                    InitializeMudObject(r);

                return r;
            }
        }

        override public RMUD.MudObject ReloadObject(string Path)
        {
            return GetObject(Path);
        }
        
        override public void PersistInstance(RMUD.MudObject Object)
        {
        }

        override public void ForgetInstance(RMUD.MudObject Object)
        {
        }

        override public Tuple<bool, string> LoadSourceFile(string Path)
        {
            return Tuple.Create(false, "The compiled database does not support this operation.");
        }

        override public int Save()
        {
            return 0;
        }

        override public void Initialize()
        {
            Core.SettingsObject = new Settings();
            var settings = GetObject("settings") as Settings;
            if (settings == null) Core.LogError("No settings object found in database. Using default settings.");
            else Core.SettingsObject = settings;
            NamedObjects.Clear();           
        }
    }
}
