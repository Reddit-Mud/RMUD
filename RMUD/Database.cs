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

        public MudObject ReLoadObject(String path)
        {
			Console.WriteLine("Loading object " + StaticPath + path + ".");
			var staticObjectPath = StaticPath + path + ".cs";

			if (System.IO.File.Exists(staticObjectPath))
			{
				var source = System.IO.File.ReadAllText(staticObjectPath);

				CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

				var parameters = new CompilerParameters();
				parameters.GenerateInMemory = true;
				parameters.GenerateExecutable = false;
				parameters.OutputAssembly = Guid.NewGuid().ToString();
				parameters.ReferencedAssemblies.Add("RMUD.exe");
				CompilerResults compilationResults = codeProvider.CompileAssemblyFromSource(parameters, source);
				if (compilationResults.Errors.Count > 0)
					throw new InvalidOperationException("Error loading object " + staticObjectPath);

				var objectLeafName = System.IO.Path.GetFileNameWithoutExtension(path);
				var newMudObject = compilationResults.CompiledAssembly.CreateInstance(objectLeafName) as MudObject;
				if (newMudObject != null)
				{
					newMudObject.Path = path;
					NamedObjects.Upsert(path, newMudObject);
					return newMudObject;
				}
				else
					throw new InvalidOperationException("Error loading object " + staticObjectPath);
			}
			else
				throw new InvalidOperationException("File does not exist - " + staticObjectPath);
		}

    }
}
