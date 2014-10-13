using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        public static String StaticPath { get; private set; }
        internal static Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();

        internal static void InitializeDatabase(String basePath)
        {
            StaticPath = basePath + "static/";
        }

        internal static String GetObjectRealPath(String Path)
        {
            return StaticPath + Path + ".cs";
        }
		
        internal static void EnumerateDatabase(String DirectoryPath, bool Recursive, Action<String> OnFile)
        {
            var path = StaticPath + DirectoryPath;
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
                if (System.IO.Path.GetExtension(file) == ".cs") 
                    OnFile(file.Substring(StaticPath.Length, file.Length - StaticPath.Length - 3).Replace("\\", "/"));
            if (Recursive)
            {
                foreach (var directory in System.IO.Directory.EnumerateDirectories(path))
                    EnumerateDatabase(directory.Substring(StaticPath.Length), true, OnFile);
            }
        }

        internal static void BulkCompile(String DirectoryPath, bool Recursive, Action<String> ReportErrors)
        {
            if (NamedObjects.Count != 1) //That is, if anything besides Settings has been loaded...
                throw new InvalidOperationException("Bulk compilation must happen before any other objects are loaded or bad things happen.");

            var source = new StringBuilder();
            source.Append("using System;\nusing System.Collections.Generic;\nusing RMUD;\nnamespace database {\n");
            int namespaceCount = 0;
            EnumerateDatabase(DirectoryPath, Recursive, (s) =>
            {
                source.AppendFormat("namespace __{0} {{\n", namespaceCount);
                namespaceCount += 1;
                source.Append(LoadSourceFile(StaticPath + s + ".cs", ReportErrors, null));
                source.Append("\n}\n\n");
            });
            source.Append("}\n");

            //System.IO.File.WriteAllText("dump.txt", source.ToString());

            var combinedAssembly = CompileCode(source.ToString(), DirectoryPath + "/*", ReportErrors);

            if (combinedAssembly != null)
            {
                namespaceCount = 0;
                EnumerateDatabase(DirectoryPath, Recursive, (s) =>
                    {
                        var qualifiedName = String.Format("database.__{0}.{1}", namespaceCount, System.IO.Path.GetFileNameWithoutExtension(s));
                        namespaceCount += 1;
                        var newObject = combinedAssembly.CreateInstance(qualifiedName) as MudObject;
                        if (newObject == null)
                        {
                            ReportErrors(String.Format("Type {0} not found in combined assembly.", qualifiedName));
                            return;
                        }

                        newObject.Path = s;
                        newObject.State = ObjectState.Unitialized;

                        NamedObjects.Upsert(s, newObject);
                    });
            }
        }

        public static MudObject GetObject(String Path, Action<String> ReportErrors = null)
        {
            Path = Path.Replace('\\', '/');

            MudObject r = null;
            
            if (NamedObjects.ContainsKey(Path)) 
                r = NamedObjects[Path];
            else 
            {
                r = LoadObject(Path, ReportErrors);
                if (r != null) NamedObjects.Upsert(Path, r);
            }

            if (r != null && r.State == ObjectState.Unitialized)
            {
                r.Initialize();
                r.State = ObjectState.Alive;
                r.HandleMarkedUpdate();
			}

			return r;
        }

        public static MudObject GetOrCreateInstance(String Path, String InstanceName, Action<String> ReportErrors = null)
		{
            Path = Path.Replace('\\', '/');
			var baseObject = GetObject(Path, ReportErrors);

			//We can't make an instance of nothing; this means that the base object has an error of some kind.
			if (baseObject == null) return null;

			//Create the new instance of the same class as the base type.
			var assembly = baseObject.GetType().Assembly;
			var newMudObject = Activator.CreateInstance(baseObject.GetType()) as MudObject;

			//It should not be possible for newMudObject to be null.
			if (newMudObject != null)
			{
				newMudObject.Path = Path;

				//The 'Get' part of GetOrCreate is some database magic - if this instance exists in the database,
				//automatically hook up to it. If not, the database should create a new entry for it.
				newMudObject.Instance = InstanceName;

				newMudObject.Initialize();
                newMudObject.State = ObjectState.Alive;
                newMudObject.HandleMarkedUpdate();
				return newMudObject;
			}
			else
			{
				throw new InvalidProgramException();
			}
		}

        public static String LoadSourceFile(String Path, Action<String> ReportErrors, List<String> FilesLoaded)
        {
            Path = Path.Replace('\\', '/');

            if (!System.IO.File.Exists(Path))
            {
                LogError(String.Format("Could not find {0}", Path));
                if (ReportErrors != null) ReportErrors("Could not find " + Path);
                return "";
            }

            if (FilesLoaded == null) FilesLoaded = new List<string>();
            else if (FilesLoaded.Contains(Path))
            {
                //LogError(String.Format("Circular reference detected in {0}", Path));
                //if (ReportErrors != null) ReportErrors(String.Format("Circular reference detected in {0}", Path));
                return "";
            }

            FilesLoaded.Add(Path);

            var rawSource = new StringBuilder();

            var file = System.IO.File.OpenText(Path);
            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                if (line.StartsWith("//import"))
                {
                    var splitAt = line.IndexOf(' ');
                    if (splitAt < 0) continue;
                    var importedFilename = line.Substring(splitAt + 1);
                    rawSource.Append(LoadSourceFile(GetObjectRealPath(importedFilename), ReportErrors, FilesLoaded));
                    rawSource.AppendLine();
                }
                else
                {
                    rawSource.Append(line);
                    rawSource.AppendLine();
                }
            }

            return rawSource.ToString();
        }

        public static Assembly CompileCode(String Source, String ErrorPath, Action<String> ReportErrors)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

            var parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.ReferencedAssemblies.Add("RMUD.exe");

            CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, Source);
            if (compilationResults.Errors.Count > 0)
            {
                var errorString = new StringBuilder();
                errorString.AppendLine(String.Format("{0} errors in {1}", compilationResults.Errors.Count, ErrorPath));

                foreach (var error in compilationResults.Errors)
                {
                    if (ReportErrors != null) ReportErrors(error.ToString());
                    errorString.Append(error.ToString());
                    errorString.AppendLine();
                }
                LogError(errorString.ToString());
                return null;
            }

            return compilationResults.CompiledAssembly;
        }

		public static Assembly CompileScript(String Path, Action<String> ReportErrors)
		{
            var start = DateTime.Now;
            Path = Path.Replace('\\', '/');

			if (!System.IO.File.Exists(Path))
			{
                LogError(String.Format("Could not find {0}", Path));
				if (ReportErrors != null) ReportErrors("Could not find " + Path);
				return null;
			}

            var source = "using System;\r\nusing System.Collections.Generic;\r\nusing RMUD;\r\n" + LoadSourceFile(Path, ReportErrors, new List<string>());

            var assembly = CompileCode(source, Path, ReportErrors);

            LogError(String.Format("Compiled {0} in {1} milliseconds.", Path, (DateTime.Now - start).TotalMilliseconds));

			return assembly;
		}

        private static MudObject LoadObject(String Path, Action<String> ReportErrors)
        {
            Path = Path.Replace('\\', '/');

            var staticObjectPath = StaticPath + Path + ".cs";
			var assembly = CompileScript(staticObjectPath, ReportErrors);
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
                LogError(String.Format("Type {0} not found in {1}", objectLeafName, staticObjectPath));
				return null;
			}
		}

		internal static MudObject ReloadObject(String Path, Action<String> ReportErrors)
		{
            Path = Path.Replace('\\', '/');

			if (NamedObjects.ContainsKey(Path))
			{
				var existing = NamedObjects[Path];
				var newObject = LoadObject(Path, ReportErrors);
				if (newObject == null) return null;

				NamedObjects.Upsert(Path, newObject);
                newObject.Initialize(); 
                newObject.State = ObjectState.Alive;
				newObject.HandleMarkedUpdate();

				//Preserve contents
				if (existing is Container && newObject is Container)
				{
                    (existing as Container).EnumerateObjects(RelativeLocations.EveryMudObject, (MudObject, loc) =>
                        {
                            (newObject as Container).Add(MudObject, loc);
                            if (MudObject is MudObject) (MudObject as MudObject).Location = newObject;
                            return EnumerateObjectsControl.Continue;
                        });
				}

				//Preserve location
				if (existing is MudObject && newObject is MudObject)
				{
					if ((existing as MudObject).Location != null)
					{
                        var loc = ((existing as MudObject).Location as Container).LocationOf(existing);
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

                var newObject = Activator.CreateInstance(existing.GetType()) as MudObject;
                NamedObjects.Upsert(Path, newObject);
                newObject.Initialize();
                newObject.State = ObjectState.Alive;
                newObject.HandleMarkedUpdate();

                //Preserve the location of actors, and actors only.
                if (existing is Container)
                {
                    (existing as Container).EnumerateObjects(RelativeLocations.EveryMudObject, (MudObject, loc) =>
                        {
                            if (MudObject is Actor)
                            {
                                //Can't use MudObject.Move - it will change the list we are iterating.
                                (newObject as Container).Add(MudObject, loc);
                                (MudObject as MudObject).Location = newObject;
                            }
                            else
                            {
                                MudObject.Destroy(true);
                            }
                            return EnumerateObjectsControl.Continue;
                        });
                }

                if (existing is MudObject && (existing as MudObject).Location != null)
                {
                    var loc = ((existing as MudObject).Location as Container).LocationOf(existing);
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
}
