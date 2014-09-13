using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public class Database
    {
        public String StaticPath { get; private set; }
        public String SerializedPath { get; private set; }
        private Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();

        public Database(String basePath)
        {
            this.StaticPath = basePath + "static/";
            this.SerializedPath = basePath + "serialized/";
        }

        public MudObject CreateObject(String path)
        {
            if (LoadObject(path) != null) return null;
            NamedObjects.Upsert(path, new MudObject{Path = path});
            return NamedObjects[path];
        }

        public MudObject CreateUniquelyNamedObject(String basePath)
        {
            while (true)
            {
                var randomPart = Guid.NewGuid();
                var path = basePath + "/" + randomPart.ToString();
                if (LoadObject(path) == null)
                {
					NamedObjects.Upsert(path, new MudObject { Path = path });
                    return NamedObjects[path];
                }
            }
        }

        public MudObject LoadObject(String path)
        {
            if (NamedObjects.ContainsKey(path)) return NamedObjects[path];
            return ReLoadObject(path);
        }

		public Assembly CompileScript(String Path)
		{
			Console.WriteLine("Compiling " + Path);

			if (!System.IO.File.Exists(Path))
			{
				Console.WriteLine("Could not find file " + Path);
				return null;
			}

			var source = System.IO.File.ReadAllText(Path);

			CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

			var parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			parameters.OutputAssembly = Guid.NewGuid().ToString();
			parameters.ReferencedAssemblies.Add("RMUD.exe");
			CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, source);
			if (compilationResults.Errors.Count > 0)
			{
				foreach (var error in compilationResults.Errors)
					Console.WriteLine(error.ToString());
				Console.WriteLine(compilationResults.Errors.Count.ToString() + " errors in " + Path);
				return null;
			}

			Console.WriteLine("Success.");
			return compilationResults.CompiledAssembly;
		}

        public MudObject ReLoadObject(String Path)
        {
			Console.WriteLine("Loading object " + StaticPath + Path + ".");

			var staticObjectPath = StaticPath + Path + ".cs";
			var assembly = CompileScript(staticObjectPath);
			if (assembly == null) return null;

			var objectLeafName = System.IO.Path.GetFileNameWithoutExtension(Path);
			var newMudObject = assembly.CreateInstance(objectLeafName) as MudObject;
			if (newMudObject != null)
			{
				newMudObject.Path = Path;
				NamedObjects.Upsert(Path, newMudObject);
				return newMudObject;
			}
			else
			{
				Console.WriteLine("Object " + objectLeafName + " not found in script " + staticObjectPath);
				return null;
			}
		}
    }
}
