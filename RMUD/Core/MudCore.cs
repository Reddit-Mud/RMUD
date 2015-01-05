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
        private static bool ShuttingDown = false;

        internal static Settings SettingsObject;

        internal static WorldDataService Database;

        public static void TiePlayerToClient(Client Client, Actor Actor)
        {
            Client.Player = Actor;
            Actor.ConnectedClient = Client;
        }

        public static void AddPlayer(Actor Actor)
        {
            Actor.Rank = 500;
            MudObject.Move(Actor, MudObject.GetObject(Core.SettingsObject.NewPlayerStartRoom));
            Core.EnqueuActorCommand(Actor, "look");
            GlobalRules.ConsiderPerformRule("player joined", Actor);
        }

        public static void RemovePlayer(Actor Actor)
        {
            GlobalRules.ConsiderPerformRule("player left", Actor);
            Actor.ConnectedClient = null;
            MudObject.Move(Actor, null);
        }

        public static bool Start(WorldDataService Database)
        {
            try
            {
                InitializeCommandProcessor();

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                    foreach (var method in type.GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                            method.Invoke(null, null);

                PersistentValueSerializer.AddGlobalSerializer(new BitArraySerializer());

                Core.Database = Database;
                Database.Initialize();


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
