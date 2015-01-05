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
        public String StaticPath { get; private set; }
        public String DynamicPath { get; private set; }
        private String UsingDeclarations = "using System;\nusing System.Collections.Generic;\nusing RMUD;\nusing System.Linq;\n";
        private System.Net.WebClient WebClient = new System.Net.WebClient();

        internal Dictionary<String, MudObject> NamedObjects = null;

        public void Initialize()
        {
            NamedObjects = new Dictionary<string, MudObject>();
            StaticPath = "database/static/";
            DynamicPath = "database/dynamic/";

            Core.SettingsObject = new Settings();
            var settings = GetObject("settings") as Settings;
            if (settings == null) Core.LogError("No settings object found in database. Using default settings.");
            else Core.SettingsObject = settings;
            NamedObjects.Clear();

            var start = DateTime.Now;
            var errorReported = false;
            InitialBulkCompile((s) =>
            {
                Core.LogError(s);
                errorReported = true;
            });

            if (errorReported) Console.WriteLine("Bulk compilation failed. Using ad-hoc compilation as fallback.");
            else
                Console.WriteLine("Total compilation in {0}.", DateTime.Now - start);

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

        private void InitialBulkCompile(Action<String> ReportErrors)
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
                if (fileList.Contains(item)) Core.LogError("Object present in github and local database: " + item);
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

                    foreach (var method in newObject.GetType().GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                            method.Invoke(null, null);

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

        public MudObject GetObject(String Path)
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
                    InitializeMudObject(r);

                return r;
            }
        }

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

        public Tuple<bool, String> LoadSourceFile(String Path)
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

        private Assembly CompileCode(String Source, String ErrorPath, Func<int,String> TranslateBulkFilenames = null)
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
            //parameters.ReferencedAssemblies.Add("System.Data.Entity.dll");

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
                Core.LogError(errorString.ToString());
            }

            if (realError) return null;
            return compilationResults.CompiledAssembly;
        }

		private MudObject CompileObject(String Path)
        {
            Path = Path.Replace('\\', '/');

            var start = DateTime.Now;
            var preprocessedFile = PreprocessSourceFile(Path, null);
            if (String.IsNullOrEmpty(preprocessedFile)) return null;

            var source = UsingDeclarations + preprocessedFile;
            var assembly = CompileCode(source, Path, i => Path);

            Core.LogError(String.Format("Compiled {0} in {1} milliseconds.", Path, (DateTime.Now - start).TotalMilliseconds));

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
                Core.LogError(String.Format("Type {0} not found in {1}", objectLeafName, Path));
				return null;
			}
		}

		public MudObject ReloadObject(String Path)
		{
            Path = Path.Replace('\\', '/');

			if (NamedObjects.ContainsKey(Path))
			{
				var existing = NamedObjects[Path];
				var newObject = CompileObject(Path);
				if (newObject == null)  return null;

                existing.State = ObjectState.Destroyed;
				NamedObjects.Upsert(Path, newObject);
                InitializeMudObject(newObject);

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
				return GetObject(Path);
		}

        public MudObject ResetObject(String Path)
        {
            Path = Path.Replace('\\', '/');

            if (NamedObjects.ContainsKey(Path))
            {
                var existing = NamedObjects[Path];
                existing.State = ObjectState.Destroyed;

                var newObject = Activator.CreateInstance(existing.GetType()) as MudObject;
                NamedObjects.Upsert(Path, newObject);
                InitializeMudObject(newObject);

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

                return newObject;
            }
            else
                return null;
        }

        private static void InitializeMudObject(MudObject Object)
        {
            Object.Initialize();
            Object.State = ObjectState.Alive;
            GlobalRules.ConsiderPerformRule("update", Object);
        }
    }
}
