using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class PlayerAttendanceRules
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject>("player joined", "[Player] : Considered when a player enters the game.", "actor");

            GlobalRules.DeclarePerformRuleBook<MudObject>("player left", "[Player] : Considered when a player leaves the game.", "actor");

            GlobalRules.Perform<MudObject>("player joined")
                .First
                .Do((actor) =>
                {
                    MudObject.Move(actor, MudObject.GetObject(Core.SettingsObject.NewPlayerStartRoom));
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Move to start room rule.");
        }
    }

    public static partial class Core
    {
        internal static Mutex DatabaseLock = new Mutex();
        public static bool ShuttingDown { get; private set; }
        public static Settings SettingsObject;
        public static String DatabasePath;
        public static WorldDataService Database;
        public static RuleEngine GlobalRules;
        public static Action OnShutDown = null;
        public static List<ModuleAssembly> IntegratedModules = new List<ModuleAssembly>();
        private static StartupFlags Flags;

        public static bool Silent { get { return (Flags & StartupFlags.Silent) == StartupFlags.Silent; } }
        public static bool NoLog { get { return (Flags & StartupFlags.NoLog) == StartupFlags.NoLog; } }

        public static void TiePlayerToClient(Client Client, MudObject Actor)
        {
            Client.Player = Actor;
            Actor.SetProperty("client", Client);
        }

        public static void AddPlayer(MudObject Actor)
        {
            Actor.SetProperty("rank", 500);
            GlobalRules.ConsiderPerformRule("player joined", Actor);
        }

        public static void RemovePlayer(MudObject Actor)
        {
            GlobalRules.ConsiderPerformRule("player left", Actor);
            Actor.SetProperty("client", null);
            MudObject.Move(Actor, null);
        }

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}
