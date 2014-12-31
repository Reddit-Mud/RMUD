using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public interface Startup
    {
        void Startup();
    }

    public static partial class Core
    {
        public static String StaticPath { get; private set; }
        public static String DynamicPath { get; private set; }
        public static String AccountsPath { get; private set; }
        public static String ChatLogsPath { get; private set; }
        private static String UsingDeclarations = "using System;\nusing System.Collections.Generic;\nusing RMUD;\nusing System.Linq;\n";
        private static System.Net.WebClient WebClient = new System.Net.WebClient();

        internal static Dictionary<String, MudObject> NamedObjects = null;

        internal static void InitializeDatabase(String basePath)
        {
            NamedObjects = new Dictionary<string, MudObject>();
            StaticPath = basePath + "static/";
            DynamicPath = basePath + "dynamic/";
            AccountsPath = basePath + "accounts/";
            ChatLogsPath = basePath + "chatlogs/";
        }

        internal static String GetObjectRealPath(String Path)
        {
            return StaticPath + Path + ".cs";
        }
		
        internal static List<String> EnumerateLocalDatabase(String DirectoryPath)
        {
            var path = StaticPath + DirectoryPath;
            var r = new List<String>();
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
                if (System.IO.Path.GetExtension(file) == ".cs")
                    r.Add(file.Substring(StaticPath.Length, file.Length - StaticPath.Length - 3).Replace("\\", "/"));
            foreach (var directory in System.IO.Directory.EnumerateDirectories(path))
                r.AddRange(EnumerateLocalDatabase(directory.Substring(StaticPath.Length)));
            return r;
        }

        internal static List<String> EnumerateGithubDatabase()
        {
            try
            {
                var githubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Reddit-Mud"));
                if (!String.IsNullOrEmpty(SettingsObject.GithubAuthToken))
                    githubClient.Credentials = new Octokit.Credentials(SettingsObject.GithubAuthToken);

                var codeSearch = new Octokit.SearchCodeRequest(".cs")
                {
                    Repo = SettingsObject.GithubRepo,
                    In = new[] { Octokit.CodeInQualifier.Path },
                    Page = 1
                };

                var fileList = new List<String>();
                Octokit.SearchCodeResult codeResult = null;
                var fileCount = 0;
                do
                {
                    codeResult = githubClient.Search.SearchCode(codeSearch).Result;
                    fileList.AddRange(codeResult.Items.Where(i => i.Path.StartsWith("static/")).Select(i => i.Path.Substring("static/".Length, i.Path.Length - "static/".Length - 3)));
                    codeSearch.Page += 1;
                    fileCount += codeResult.Items.Count;
                } while (fileCount < codeResult.TotalCount);

                return new List<string>(fileList.Distinct());
            }
            catch (Exception e)
            {
                Console.WriteLine("Github filelist discovery failed. Only startup objects present in local database will be loaded.");
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }

        private class FileTableEntry
        {
            public String Path;
            public int FirstLine;
        }

        private static String PathToNamespace(String Path)
        {
            return "__" + Path.Replace("/", "_").Replace("-", "_");
        }

        internal static void InitialBulkCompile(Action<String> ReportErrors)
        {
            if (NamedObjects.Count != 0) //That is, if anything besides Settings has been loaded...
                throw new InvalidOperationException("Bulk compilation must happen before any other objects are loaded or bad things happen.");

            var fileTable = new List<FileTableEntry>();
            var source = new StringBuilder();
            source.Append(UsingDeclarations + "namespace database {\n");
            int lineCount = 5;

            var fileList = EnumerateGithubDatabase();
            foreach (var item in EnumerateLocalDatabase(""))
            {
                if (fileList.Contains(item)) LogError("Object present in github and local database: " + item);
                else fileList.Add(item);
            }

            foreach (var s in fileList)
            {
                fileTable.Add(new FileTableEntry { Path = s, FirstLine = lineCount });
                lineCount += 4;
                source.AppendFormat("namespace {0} {{\n", PathToNamespace(s));
                var fileSource = PreprocessSourceFile(s, null);
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
                    var newObject = combinedAssembly.CreateInstance(qualifiedName) as MudObject;
                    if (newObject == null)
                    {
                        ReportErrors(String.Format("Type {0} not found in combined assembly.", qualifiedName));
                        return;
                    }

                    newObject.Path = s;
                    newObject.State = ObjectState.Unitialized;

                    if (newObject is Startup) (newObject as Startup).Startup();

                    NamedObjects.Upsert(s, newObject);
                }
            }
        }

        public static void SplitObjectName(String FullName, out String BasePath, out String InstanceName)
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

        public static MudObject GetObject(String Path, Action<String> ReportErrors = null)
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
                    r = CompileObject(BasePath, ReportErrors);
                    if (r != null) NamedObjects.Upsert(BasePath, r);
                }

                if (r != null && r.State == ObjectState.Unitialized)
                {
                    r.Initialize();
                    r.State = ObjectState.Alive;
                    r.HandleMarkedUpdate();
                }

                return r;
            }
        }

        private static String PreprocessSourceFile(String Path, List<String> FilesLoaded = null)
        {
            Path = Path.Replace('\\', '/');

            var source = LoadSourceFile(Path);
            if (source.Item1 == false)
            {
                LogError(Path + " - " + source.Item2);
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

        public static Tuple<bool, String> LoadSourceFile(String Path)
        {
            Path = Path.Replace('\\', '/');
            if (Path.Contains("..")) return Tuple.Create(false, "Backtrack path entries are not permitted.");

            if (SettingsObject.UseGithubDatabase)
            {
                try
                {
                    return Tuple.Create(true, WebClient.DownloadString(SettingsObject.GithubRawURL + Path + ".cs"));
                }
                catch (Exception) { }
            }

            var realPath = StaticPath + Path + ".cs";

            if (!System.IO.File.Exists(realPath)) return Tuple.Create(false, "File not found.");
            return Tuple.Create(true, System.IO.File.ReadAllText(realPath));
        }

        public static Assembly CompileCode(String Source, String ErrorPath, Func<int,String> TranslateBulkFilenames = null)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

            var parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.ReferencedAssemblies.Add("RMUD.exe");

            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
            parameters.ReferencedAssemblies.Add("System.Data.Entity.dll");

            CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, Source);
            bool realError = false;
            if (compilationResults.Errors.Count > 0)
            {
                var errorString = new StringBuilder();
                errorString.AppendLine(String.Format("{0} errors in {1}", compilationResults.Errors.Count, ErrorPath));

                foreach (var error in compilationResults.Errors)
                {
                    var cError = error as System.CodeDom.Compiler.CompilerError;
                    if (!cError.IsWarning) realError = true;

                    var filename = cError.FileName;
                    if (TranslateBulkFilenames != null)
                        filename = TranslateBulkFilenames(cError.Line);

                    errorString.Append(filename + " : " + error.ToString());
                    errorString.AppendLine();
                }
                LogError(errorString.ToString());
            }

            if (realError) return null;
            return compilationResults.CompiledAssembly;
        }

		private static MudObject CompileObject(String Path, Action<String> ReportErrors)
        {
            Path = Path.Replace('\\', '/');

            var start = DateTime.Now;
            var preprocessedFile = PreprocessSourceFile(Path, null);
            if (String.IsNullOrEmpty(preprocessedFile)) return null;

            var source = UsingDeclarations + preprocessedFile;
            var assembly = CompileCode(source, Path, i => Path);

            LogError(String.Format("Compiled {0} in {1} milliseconds.", Path, (DateTime.Now - start).TotalMilliseconds));

			if (assembly == null) return null;

			var objectLeafName = System.IO.Path.GetFileNameWithoutExtension(Path);
			var newMudObject = assembly.CreateInstance(objectLeafName) as MudObject;
			if (newMudObject != null)
			{
				newMudObject.Path = Path;
                newMudObject.State = ObjectState.Unitialized;
				return newMudObject;
			}
			else
			{
                LogError(String.Format("Type {0} not found in {1}", objectLeafName, Path));
				return null;
			}
		}

		internal static MudObject ReloadObject(String Path, Action<String> ReportErrors)
		{
            Path = Path.Replace('\\', '/');

			if (NamedObjects.ContainsKey(Path))
			{
				var existing = NamedObjects[Path];
				var newObject = CompileObject(Path, ReportErrors);
				if (newObject == null)  return null;

                existing.State = ObjectState.Destroyed;
				NamedObjects.Upsert(Path, newObject);
                newObject.Initialize(); 
                newObject.State = ObjectState.Alive;
				newObject.HandleMarkedUpdate();

				//Preserve contents
				if (existing is Container && newObject is Container)
                    foreach (var item in (existing as Container).EnumerateObjectsAndRelloc())
                    {
                        (newObject as Container).Add(item.Item1, item.Item2);
                        item.Item1.Location = newObject;
                    }
                 
				//Preserve location
				if (existing is MudObject && newObject is MudObject)
				{
					if ((existing as MudObject).Location != null)
					{
                        var loc = ((existing as MudObject).Location as Container).RelativeLocationOf(existing);
						MudObject.Move(newObject as MudObject, (existing as MudObject).Location, loc);
						MudObject.Move(existing as MudObject, null, RelativeLocations.None);
					}
				}

                existing.Destroy(false);

				return newObject;
			}
			else
				return GetObject(Path, ReportErrors);
		}

        internal static bool ResetObject(String Path, Action<String> ReportErrors)
        {
            Path = Path.Replace('\\', '/');

            if (NamedObjects.ContainsKey(Path))
            {
                var existing = NamedObjects[Path];
                existing.State = ObjectState.Destroyed;
                
                var newObject = Activator.CreateInstance(existing.GetType()) as MudObject;
                NamedObjects.Upsert(Path, newObject);
                newObject.Initialize();
                newObject.State = ObjectState.Alive;
                newObject.HandleMarkedUpdate();

                //Preserve the location of actors, and actors only.
                if (existing is Container)
                    foreach (var item in (existing as Container).EnumerateObjectsAndRelloc())
                        if (item.Item1 is Actor)
                        {
                            (newObject as Container).Add(item.Item1, item.Item2);
                            item.Item1.Location = newObject;
                        }
                    
                if (existing is MudObject && (existing as MudObject).Location != null)
                {
                    var loc = ((existing as MudObject).Location as Container).RelativeLocationOf(existing);
                    MudObject.Move(newObject as MudObject, (existing as MudObject).Location, loc);
                    MudObject.Move(existing as MudObject, null, RelativeLocations.None);
                }

                existing.Destroy(false);

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public partial class MudObject
    {
        public static MudObject GetObject(String Path)
        {
            return Core.GetObject(Path);
        }
    }
}
