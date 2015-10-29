using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public enum StartupFlags
    {
        Silent = 1,
        SearchDirectory = 2,
        SingleThreaded = 4,
        NoLog = 8
    }

    public static partial class Core
    {
        /// <summary>
        /// Integrate a module into the running game by calling every static AtStartup method found on any type
        /// in the module's base namespace.
        /// </summary>
        /// <param name="Module"></param>
        private static void IntegrateModule(ModuleAssembly Module)
        {
            if (Module == null) throw new InvalidOperationException("Tried to load null module");
            if (Module.Assembly == null) throw new InvalidOperationException("Tried to load invalid module assembly - " + Module.FileName);
            if (Module.Info == null) throw new InvalidOperationException("Tried to load invalid module assembly - " + Module.FileName);

            DefaultParser.ModuleBeingInitialized = Module.FileName;

            foreach (var type in Module.Assembly.GetTypes())
                if (type.FullName.StartsWith(Module.Info.BaseNameSpace))
                    foreach (var method in type.GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                            try
                            {
                                method.Invoke(null, new Object[] { GlobalRules });
                            }
                            catch (Exception e)
                            {
                                LogWarning("Error while loading module " + Module.FileName + " : " + e.Message);
                            }
        }

        /// <summary>
        /// Start the mud engine.
        /// </summary>
        /// <param name="Flags">Flags control engine functions</param>
        /// <param name="Database"></param>
        /// <param name="Assemblies">Modules to integrate</param>
        /// <returns></returns>
        public static bool Start(StartupFlags Flags, WorldDataService Database, params ModuleAssembly[] Assemblies)
        {
            ShuttingDown = false;
            Core.Flags = Flags;

            try
            {
                // Setup the rule engine and some basic rules.
                GlobalRules = new RuleEngine();
                GlobalRules.DeclarePerformRuleBook("at startup", "[] : Considered when the engine is started.");
                GlobalRules.DeclarePerformRuleBook<MudObject>("singleplayer game started", "Considered when a single player game is begun");

                DefaultParser = new CommandParser();

                // Integrate modules. The Core assembly is always integrated.
                IntegratedModules.Add(new ModuleAssembly(Assembly.GetExecutingAssembly(), new ModuleInfo { Author = "Blecki", Description = "RMUD Core", BaseNameSpace = "RMUD" }, "Core.dll"));
                IntegratedModules.AddRange(Assemblies);

                if ((Flags & StartupFlags.SearchDirectory) == StartupFlags.SearchDirectory)
                {
                    foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Directory.GetCurrentDirectory()).Where(p => System.IO.Path.GetExtension(p) == ".dll"))
                    {
                        var assembly = System.Reflection.Assembly.LoadFrom(file);

                        var infoType = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(ModuleInfo)));
                        if (infoType != null)
                        {
                            IntegratedModules.Add(new ModuleAssembly(assembly, file));
                            if ((Flags & StartupFlags.Silent) == 0)
                                Console.WriteLine("Discovered module: " + file);
                        }
                    }
                }

                foreach (var startupAssembly in IntegratedModules)
                    IntegrateModule(startupAssembly);

                PersistentValueSerializer.AddGlobalSerializer(new BitArraySerializer());

                InitializeCommandProcessor();

                GlobalRules.FinalizeNewRules();

                Core.Database = Database;
                Database.Initialize();

                GlobalRules.ConsiderPerformRule("at startup");

                if ((Flags & StartupFlags.SingleThreaded) == 0)
                    StartThreadedCommandProcesor();
            }
            catch (Exception e)
            {
                LogError("Failed to start mud engine.");
                LogError(e.Message);
                LogError(e.StackTrace);
                throw;
            }
            return true;
        }
    }
}
