using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Core
    {
        internal static Mutex DatabaseLock = new Mutex();
        public static bool ShuttingDown { get; private set; }
        public static Settings SettingsObject;
        public static WorldDataService Database;
        public static RuleEngine GlobalRules;
        public static Action OnShutDown = null;
        public static List<ModuleAssembly> ModuleAssemblies = new List<ModuleAssembly>();
        private static StartupFlags Flags;

        public static bool Silent { get { return (Flags & StartupFlags.Silent) == StartupFlags.Silent; } }
        public static bool NoLog { get { return (Flags & StartupFlags.NoLog) == StartupFlags.NoLog; } }

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

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}
