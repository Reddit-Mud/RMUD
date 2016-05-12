using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class RuntimeDatabase : WorldDataService
    {
        override public Tuple<bool, String> LoadSourceFile(String Path)
        {
            Path = Path.Replace('\\', '/');
            if (Path.Contains("..")) return Tuple.Create(false, "Backtrack path entries are not permitted.");

            if (Core.SettingsObject.UseGithubDatabase)
            {
                try
                {
                    return Tuple.Create(true, WebClient.DownloadString(Core.SettingsObject.GithubRawURL + Path + ".cs"));
                }
                catch (Exception) { }
            }

            var realPath = StaticPath + Path + ".cs";

            if (!System.IO.File.Exists(realPath)) return Tuple.Create(false, "File not found.");
            return Tuple.Create(true, System.IO.File.ReadAllText(realPath));
        }
    }
}
