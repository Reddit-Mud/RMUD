using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Raven.Client.Embedded;

namespace RMUD
{
    public static partial class Mud
    {
        public static String StaticPath { get; private set; }
        public static String PersistentPath { get; private set; }
        public static String SerializedPath { get; private set; }
        internal static Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();
        public static EmbeddableDocumentStore PersistentStore { get; private set; }

        internal static void InitializeDatabase(String basePath)
        {
            StaticPath = basePath + "static/";
            SerializedPath = basePath + "serialized/";
            PersistentPath = basePath + "persistent/";
            PersistentStore = new EmbeddableDocumentStore();// { DataDirectory = PersistentPath.Replace("\\","/") };
            PersistentStore.Initialize();
        }

        public static MudObject GetObject(String id, Action<String> ReportErrors = null)
        {
            if (NamedObjects.ContainsKey(id)) return NamedObjects[id];
			
			var result = LoadObject(id, ReportErrors);
			if (result != null)
			{
				NamedObjects.Upsert(id, result);
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

        private static MudObject LoadObject(String id, Action<String> ReportErrors)
        {
			Console.WriteLine("Loading object " + StaticPath + id);

			var staticObjectPath = StaticPath + id + ".cs";
			var assembly = CompileScript(staticObjectPath, ReportErrors);
			if (assembly == null) return null;

			var objectLeafName = System.IO.Path.GetFileNameWithoutExtension(id);
			var newMudObject = assembly.CreateInstance(objectLeafName) as MudObject;
			if (newMudObject != null)
			{
				newMudObject.Id = id;
				return newMudObject;
			}
			else
			{
				Console.WriteLine("Object " + objectLeafName + " not found in script " + staticObjectPath);
				return null;
			}
		}

		internal static MudObject ReloadObject(String id, Action<String> ReportErrors)
		{
			if (NamedObjects.ContainsKey(id))
			{
				var newObject = LoadObject(id, ReportErrors);
				if (newObject == null) return null;
                var existing = NamedObjects[id];

				NamedObjects.Upsert(id, newObject);
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
				return GetObject(id, ReportErrors);
		}
    }
}
