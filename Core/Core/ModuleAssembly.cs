using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    /// <summary>
    /// Represents an assembly that should be integrated at startup. Core.Start will search an assembly
    /// for every type that implements 'static void OnStartup(RuleEngine)' and try to call it.
    /// A module can use this behavior to implement registration of it's custom rules.
    /// </summary>
    public class ModuleAssembly
    {
        public Assembly Assembly;
        public String FileName;
        public ModuleInfo Info;

        /// <summary>
        /// Construct a module assembly from an assembly
        /// </summary>
        /// <param name="Assembly"></param>
        /// <param name="Info"></param>
        /// <param name="FileName"></param>
        public ModuleAssembly(Assembly Assembly, ModuleInfo Info, String FileName = "")
        {
            this.Assembly = Assembly;
            this.Info = Info;
            this.FileName = FileName;
        }

        /// <summary>
        /// Construct a module assembly from an assembly. Automatically find and load the ModuleInfo type
        /// for the module assembly.
        /// </summary>
        /// <param name="Assembly"></param>
        /// <param name="FileName"></param>
        public ModuleAssembly(Assembly Assembly, String FileName)
        {
            this.Assembly = Assembly;
            var InfoType = Assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(ModuleInfo)));
            Info = Activator.CreateInstance(InfoType) as ModuleInfo;
            if (Info == null) throw new InvalidOperationException("Specified assembly is not a module.");
            this.FileName = FileName;
        }

        /// <summary>
        /// Construct a module assembly from an assembly on disc. Automatically find and load the ModuleInfo
        /// type.
        /// </summary>
        /// <param name="FileName">The assembly file to load</param>
        public ModuleAssembly(String FileName)
        {
            FileName = System.IO.Path.GetFullPath(FileName);
            this.FileName = FileName;

            Assembly = System.Reflection.Assembly.LoadFrom(FileName);
            if (Assembly == null) throw new InvalidOperationException("Could not load assembly " + FileName);

            Info = Activator.CreateInstance(Assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(ModuleInfo)))) as ModuleInfo;
            if (Info == null) throw new InvalidOperationException("Specified assembly is not a module.");
        }
    }
}
