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
        private void InitialBulkCompile(Action<String> ReportErrors)
        {
            if (NamedObjects.Count != 0) //That is, if anything besides Settings has been loaded...
                throw new InvalidOperationException("Bulk compilation must happen before any other objects are loaded or bad things happen.");

            var fileTable = new List<FileTableEntry>();
            var source = new StringBuilder();
            source.Append(GetFileHeader() + "namespace database {\n");
            int lineCount = 5;

            List<String> fileList = null;

            if (Core.SettingsObject.UseGithubDatabase)
                fileList = EnumerateGithubDatabase();
            else
                fileList = new List<String>();

            foreach (var item in EnumerateLocalDatabase(""))
            {
                if (fileList.Contains(item)) Core.LogError("Object present in github and local database: " + item);
                else fileList.Add(item);
            }

            foreach (var s in fileList)
            {
                fileTable.Add(new FileTableEntry { Path = s, FirstLine = lineCount });
                lineCount += 4;
                source.AppendFormat("namespace {0} {{\n", PathToNamespace(s));
                var fileSource = PreprocessSourceFile(s);
                lineCount += fileSource.Count(c => c == '\n');
                source.Append(fileSource);
                source.Append("\n}\n\n");

            }

            source.Append("}\n");

            var combinedAssembly = CompileCode(source.ToString(), "/*", i =>
                {
                    var r = fileTable.Reverse<FileTableEntry>().FirstOrDefault(e => e.FirstLine < i);
                    if (r != null) return r.Path;
                    return "";
                });

            if (combinedAssembly != null)
            {
                foreach (var s in fileList)
                {
                    var qualifiedName = String.Format("database.{0}.{1}", PathToNamespace(s), System.IO.Path.GetFileNameWithoutExtension(s));
                    var newObject = combinedAssembly.CreateInstance(qualifiedName);
                    if (newObject == null)
                    {
                        ReportErrors(String.Format("Type {0} not found in combined assembly.", qualifiedName));
                    }
                    else if (!(newObject is MudObject))
                    {
                        ReportErrors(String.Format("Type {0} was found, but was not a mud object.", qualifiedName));
                    }
                    else
                    {
                        var mudObject = newObject as MudObject;
                        mudObject.Path = s;
                        mudObject.State = ObjectState.Unitialized;

                        foreach (var method in newObject.GetType().GetMethods())
                            if (method.IsStatic && method.Name == "AtStartup")
                                method.Invoke(null, new Object[] { Core.GlobalRules });

                        NamedObjects.Upsert(s, mudObject);
                    }
                }
            }
        }
    }
}
