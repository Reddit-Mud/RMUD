using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public interface WorldDataService
    {
        MudObject GetObject(String Path);
        MudObject ReloadObject(String Path);
        MudObject ResetObject(String Path);

        void PersistInstance(MudObject Object);
        void ForgetInstance(MudObject Object);

        Tuple<bool, String> LoadSourceFile(String Path);

        int Save();

        void Initialize(String BasePath);
    }
}
