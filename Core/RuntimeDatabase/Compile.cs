﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class RuntimeDatabase : WorldDataService
    {
        private String GetFileHeader()
        {
            var builder = new StringBuilder();
            builder.Append("using System;\nusing System.Collections.Generic;\nusing System.Linq;\nusing SharpRuleEngine;\nusing RMUD;\n");

            foreach (var module in Core.IntegratedModules)
                builder.AppendLine("using " + module.Info.BaseNameSpace + ";\n");

            return builder.ToString();
        }

        private Assembly CompileCode(String Source, String ErrorPath, Func<int,String> TranslateBulkFilenames = null)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

            var parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
            parameters.ReferencedAssemblies.Add("SharpRuleEngine.dll");

            foreach (var module in Core.IntegratedModules)
                if (!String.IsNullOrEmpty(module.FileName))
                    parameters.ReferencedAssemblies.Add(module.FileName);

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
            var preprocessedFile = PreprocessSourceFile(Path);
            if (String.IsNullOrEmpty(preprocessedFile)) return null;

            var source = GetFileHeader() + preprocessedFile;
            var assembly = CompileCode(source, Path, i => Path);

            Core.LogError(String.Format("Compiled {0} in {1} milliseconds.", Path, (DateTime.Now - start).TotalMilliseconds));

			if (assembly == null) return null;

			var objectLeafName = System.IO.Path.GetFileNameWithoutExtension(Path);
			var newMudObject = Activator.CreateInstance(assembly.GetType(objectLeafName)) as MudObject;
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
    }
}
