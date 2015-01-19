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
        private String ResolveImports(String Path, List<String> FilesLoaded = null)
        {
            Path = Path.Replace('\\', '/');
            if (FilesLoaded == null) FilesLoaded = new List<String>();
            else if (FilesLoaded.Contains(Path))
                return "";
            FilesLoaded.Add(Path);

            var source = LoadSourceFile(Path);
            if (source.Item1 == false)
            {
                Core.LogError(Path + " - " + source.Item2);
                return "";
            }            

            var output = new StringBuilder();
            var stream = new System.IO.StringReader(source.Item2);
            while (true)
            {
                var line = stream.ReadLine();
                if (line == null) break;

                if (line.StartsWith("//import "))
                {
                    var importedFilename = line.Substring("//import ".Length).Trim();
                    output.Append(ResolveImports(importedFilename, FilesLoaded));
                    output.AppendLine();
                }
                else
                    output.AppendLine(line);
            }

            return output.ToString();
        }

        private String PreprocessSourceFile(String Path)
        {
            Path = Path.Replace('\\', '/');
            var source = ResolveImports(Path);
            var processedSource = MudObjectTransformTool.Pattern.ProcessFile(source);
            return processedSource;
        }
    }
}
