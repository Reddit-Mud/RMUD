using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class GithubDatabase : WorldDataService
    {
        override public MudObject GetObject(String Path)
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
                    r = CompileObject(BasePath);
                    if (r != null) NamedObjects.Upsert(BasePath, r);
                }

                if (r != null && r.State == ObjectState.Unitialized)
                    MudObject.InitializeObject(r);

                return r;
            }
        }
    }
}
