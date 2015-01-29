using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class StartUpAssembly
    {
        public Assembly Assembly;
        public String FileName;
        public ModuleInfo Info;

        public StartUpAssembly(Assembly Assembly, ModuleInfo Info, String FileName = "")
        {
            this.Assembly = Assembly;
            this.Info = Info;
            this.FileName = FileName;
        }
    }

    public static partial class Core
    {
        internal static Mutex DatabaseLock = new Mutex();
        public static bool ShuttingDown { get; private set; }
        public static Settings SettingsObject;
        public static WorldDataService Database;
        public static RuleEngine GlobalRules;
        public static Action OnShutDown = null;
        public static List<StartUpAssembly> ModuleAssemblies = new List<StartUpAssembly>();

        public static void TiePlayerToClient(Client Client, Actor Actor)
        {
            Client.Player = Actor;
            Actor.ConnectedClient = Client;
        }

        public static void AddPlayer(Actor Actor)
        {
            Actor.Rank = 500;
            GlobalRules.ConsiderPerformRule("player joined", Actor);
        }

        public static void RemovePlayer(Actor Actor)
        {
            GlobalRules.ConsiderPerformRule("player left", Actor);
            Actor.ConnectedClient = null;
            MudObject.Move(Actor, null);
        }

        private static void LoadStartupAssembly(StartUpAssembly StartUp)
        {
            foreach (var type in StartUp.Assembly.GetTypes())
                if (type.FullName.StartsWith(StartUp.Info.BaseNameSpace))
                    foreach (var method in type.GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                            method.Invoke(null, new Object[]{GlobalRules});
        }

        public static bool Start(WorldDataService Database, params StartUpAssembly[] Assemblies)
        {
            ShuttingDown = false;

            try
            {
                GlobalRules = new RuleEngine();


                GlobalRules.DeclarePerformRuleBook("at startup", "[] : Considered when the engine is started.");

                ModuleAssemblies.Add(new StartUpAssembly(Assembly.GetExecutingAssembly(), new ModuleInfo { Author = "Blecki", Description = "RMUD Core", BaseNameSpace = "RMUD" }, "Core.dll"));
                ModuleAssemblies.AddRange(Assemblies);

                    foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Directory.GetCurrentDirectory()).Where(p => System.IO.Path.GetExtension(p) == ".dll"))
                    {
                        var assembly = System.Reflection.Assembly.LoadFrom(file);

                        var info = assembly.CreateInstance("ModuleInfo") as ModuleInfo;
                        if (info != null)
                        {
                            ModuleAssemblies.Add(new StartUpAssembly(assembly, info, file));
                            Console.WriteLine("Discovered module: " + file + " : " + info.Description);
                        }
                    }

                foreach (var startupAssembly in ModuleAssemblies)
                    LoadStartupAssembly(startupAssembly);

                PersistentValueSerializer.AddGlobalSerializer(new BitArraySerializer());

                InitializeCommandProcessor();

                GlobalRules.FinalizeNewRules();

                Core.Database = Database;
                Database.Initialize();

                GlobalRules.ConsiderPerformRule("at startup");
                StartCommandProcesor();
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

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}
