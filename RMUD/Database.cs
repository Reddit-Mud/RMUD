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
        public static String SerializedPath { get; private set; }
        internal static Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();

        internal static void InitializeDatabase(String basePath)
        {
            StaticPath = basePath + "static/";
            SerializedPath = basePath + "serialized/";
        }
		
        public static MudObject GetObject(String Path, Action<String> ReportErrors = null)
        {
            if (NamedObjects.ContainsKey(Path)) return NamedObjects[Path];
			
			var result = LoadObject(Path, ReportErrors);
			if (result != null)
			{
				NamedObjects.Upsert(Path, result);
				result.Initialize();
			}
			return result;
        }

		public static Assembly CompileScript(String Path, Action<String> ReportErrors)
		{
			Console.WriteLine("Compiling " + Path);

			if (!System.IO.File.Exists(Path))
			{
				Console.WriteLine("Could not find file " + Path);
				if (ReportErrors != null) ReportErrors("Could not find file " + Path);
				return null;
			}

			var source = System.IO.File.ReadAllText(Path);

			CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

			var parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			//parameters.OutputAssembly = Guid.NewGuid().ToString();
			parameters.ReferencedAssemblies.Add("RMUD.exe");
			CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, source);
			if (compilationResults.Errors.Count > 0)
			{
				foreach (var error in compilationResults.Errors)
				{
					Console.WriteLine(error.ToString());
					if (ReportErrors != null) ReportErrors(error.ToString());
				}
				Console.WriteLine(compilationResults.Errors.Count.ToString() + " errors in " + Path);
				return null;
			}

			Console.WriteLine("Success.");
			return compilationResults.CompiledAssembly;
		}

        private static MudObject LoadObject(String Path, Action<String> ReportErrors)
        {
			Console.WriteLine("Loading object " + StaticPath + Path);

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
				Console.WriteLine("Object " + objectLeafName + " not found in script " + staticObjectPath);
				return null;
			}
		}

		internal static MudObject ReloadObject(String Path, Action<String> ReportErrors)
		{
			if (NamedObjects.ContainsKey(Path))
			{
				var existing = NamedObjects[Path];
				var newObject = LoadObject(Path, ReportErrors);
				if (newObject == null) return null;

				NamedObjects.Upsert(Path, newObject);
				newObject.Initialize();

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
    }
}
