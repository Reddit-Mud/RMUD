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
		
        internal static void EnumerateDatabase(String DirectoryPath, bool Recursive, Action<String> OnFile)
        {
            var path = StaticPath + DirectoryPath;
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
                if (System.IO.Path.GetExtension(file) == ".cs") 
                    OnFile(file.Substring(StaticPath.Length, file.Length - StaticPath.Length - 3));
            if (Recursive)
            {
                foreach (var directory in System.IO.Directory.EnumerateDirectories(path))
                    EnumerateDatabase(directory.Substring(StaticPath.Length), true, OnFile);
            }
        }

        public static MudObject GetObject(String Path, Action<String> ReportErrors = null)
        {
            Path = Path.Replace('\\', '/');
            if (NamedObjects.ContainsKey(Path)) return NamedObjects[Path];
			
			var result = LoadObject(Path, ReportErrors);
			if (result != null)
			{
				NamedObjects.Upsert(Path, result);
				result.Initialize();
                Mud.MarkChangedObject(result);
			}
			return result;
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
				return newMudObject;
			}
			else
			{
				throw new InvalidProgramException();
			}
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

			var source = "using System;\r\nusing System.Collections.Generic;\r\nusing RMUD;\r\n" + System.IO.File.ReadAllText(Path);

			CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

			var parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			//parameters.OutputAssembly = Guid.NewGuid().ToString();
			parameters.ReferencedAssemblies.Add("RMUD.exe");
			CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, source);
			if (compilationResults.Errors.Count > 0)
			{
                var errorString = new StringBuilder();
                errorString.AppendLine(String.Format("{0} errors in {1}", compilationResults.Errors.Count, Path));

				foreach (var error in compilationResults.Errors)
				{
					if (ReportErrors != null) ReportErrors(error.ToString());
                    errorString.Append(error.ToString());
                    errorString.AppendLine();
				}
		        LogError(errorString.ToString());
				return null;
			}

            LogError(String.Format("Compiled {0} in {1} milliseconds.", Path, (DateTime.Now - start).TotalMilliseconds));
			return compilationResults.CompiledAssembly;
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
                Mud.MarkChangedObject(newObject);

				//Preserve contents
				if (existing is IContainer && newObject is IContainer)
				{
					foreach (var thing in (existing as IContainer))
					{
						(newObject as IContainer).Add(thing);
						thing.Location = newObject;
					}
				}

				//Preserve location
				if (existing is Thing && newObject is Thing)
				{
					if ((existing as Thing).Location != null)
					{
						Thing.Move(newObject as Thing, (existing as Thing).Location);
						Thing.Move(existing as Thing, null);
					}
				}

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
                Mud.MarkChangedObject(newObject);

                //Preserve the location of actors, and actors only.
                if (existing is IContainer)
                {
                    foreach (var thing in (existing as IContainer))
                    {
                        if (thing is Actor)
                        {
                            //Can't use Thing.Move - it will change the list we are iterating.
                            (newObject as IContainer).Add(thing);
                            thing.Location = newObject;
                        }
                    }
                }

                if (existing is Thing && (existing as Thing).Location != null)
                {
                    Thing.Move(newObject as Thing, (existing as Thing).Location);
                    Thing.Move(existing as Thing, null);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
