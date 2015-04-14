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
        SingleThreaded = 4
    }

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

        public StartUpAssembly(Assembly Assembly)
        {
            this.Assembly = Assembly;
            Info = Assembly.CreateInstance("ModuleInfo") as ModuleInfo;
            if (Info == null) throw new InvalidOperationException("Specified assembly is not a module.");
        }

        public StartUpAssembly(String FileName)
        {
            FileName = System.IO.Path.GetFullPath(FileName);
            this.FileName = FileName;

            Assembly = System.Reflection.Assembly.LoadFrom(FileName);
            if (Assembly == null) throw new InvalidOperationException("Could not load assembly " + FileName);

            Info = Assembly.CreateInstance("ModuleInfo") as ModuleInfo;
            if (Info == null) throw new InvalidOperationException("Specified assembly is not a module.");
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
            if (StartUp == null) throw new InvalidOperationException("Tried to load null startup assembly");
            if (StartUp.Assembly == null) throw new InvalidOperationException("Tried to load invalid startup assembly - " + StartUp.FileName);
            if (StartUp.Info == null) throw new InvalidOperationException("Tried to load invalid startup assembly - " + StartUp.FileName);

            foreach (var type in StartUp.Assembly.GetTypes())
                if (type.FullName.StartsWith(StartUp.Info.BaseNameSpace))
                    foreach (var method in type.GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                            method.Invoke(null, new Object[]{GlobalRules});
        }

        public static bool Start(StartupFlags Flags, WorldDataService Database, params StartUpAssembly[] Assemblies)
        {
            ShuttingDown = false;

            try
            {
                GlobalRules = new RuleEngine(NewRuleQueueingMode.QueueNewRules);


                GlobalRules.DeclarePerformRuleBook("at startup", "[] : Considered when the engine is started.");
                GlobalRules.DeclarePerformRuleBook<MudObject>("singleplayer game started", "Considered when a single player game is begun");

                ModuleAssemblies.Add(new StartUpAssembly(Assembly.GetExecutingAssembly(), new ModuleInfo { Author = "Blecki", Description = "RMUD Core", BaseNameSpace = "RMUD" }, "Core.dll"));
                ModuleAssemblies.AddRange(Assemblies);

                if ((Flags & StartupFlags.SearchDirectory) == StartupFlags.SearchDirectory)
                {
                    foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Directory.GetCurrentDirectory()).Where(p => System.IO.Path.GetExtension(p) == ".dll"))
                    {
                        var assembly = System.Reflection.Assembly.LoadFrom(file);

                        var info = assembly.CreateInstance("ModuleInfo") as ModuleInfo;
                        if (info != null)
                        {
                            ModuleAssemblies.Add(new StartUpAssembly(assembly, info, file));
                            if ((Flags & StartupFlags.Silent) == 0)
                                Console.WriteLine("Discovered module: " + file + " : " + info.Description);
                        }
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

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}
