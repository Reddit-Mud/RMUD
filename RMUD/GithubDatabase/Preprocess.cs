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
        private String PreprocessSourceFile(String Path, List<String> FilesLoaded = null)
        {
            Path = Path.Replace('\\', '/');

            var source = LoadSourceFile(Path);
            if (source.Item1 == false)
            {
                Core.LogError(Path + " - " + source.Item2);
                return "";
            }

            if (FilesLoaded == null) FilesLoaded = new List<String>();
            else if (FilesLoaded.Contains(Path))
                return "";

            FilesLoaded.Add(Path);

            var output = new StringBuilder();
            var stream = new System.IO.StringReader(source.Item2);
            while (true)
            {
                var line = stream.ReadLine();
                if (line == null) break;

                if (line.StartsWith("//import "))
                {
                    var importedFilename = line.Substring("//import ".Length).Trim();
                    output.Append(PreprocessSourceFile(importedFilename, FilesLoaded));
                    output.AppendLine();
                }
                else
                    output.AppendLine(line);
            }

            return output.ToString();
        }
    }
}
