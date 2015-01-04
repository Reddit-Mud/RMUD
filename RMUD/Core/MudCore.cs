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
        internal static List<Client> ConnectedClients = new List<Client>();
        internal static Mutex DatabaseLock = new Mutex();
        private static bool ShuttingDown = false;

        internal static Settings SettingsObject;
        internal static ProscriptionList ProscriptionList;

        internal static WorldDataService Database;

        internal static void ClientDisconnected(Client client)
        {
            DatabaseLock.WaitOne();
            ConnectedClients.Remove(client);
            if (client.Player != null)
            {
                GlobalRules.ConsiderPerformRule("player left", client.Player);
                client.Player.ConnectedClient = null;
                client.Account.LoggedInCharacter = null;
                MudObject.Move(client.Player, null);
                //client.Player.Destroy(true);
            }
            DatabaseLock.ReleaseMutex();
        }

        internal enum ClientAcceptanceStatus
        {
            Accepted,
            Rejected,
        }

        internal static ClientAcceptanceStatus ClientConnected(Client Client)
        {
            var ban = ProscriptionList.IsBanned(Client.IPString);
            if (ban.Banned)
            {
                LogError("Rejected connection from " + Client.IPString + ". Matched ban " + ban.SourceBan.Glob + " Reason: " + ban.SourceBan.Reason);
                return ClientAcceptanceStatus.Rejected;
            }

            DatabaseLock.WaitOne();

            Client.Player = new Actor();
            Client.Player.CommandHandler = SettingsObject.NewClientCommandHandler;
            Client.Player.ConnectedClient = Client;

            MudObject.SendMessage(Client, SettingsObject.Banner);
            MudObject.SendMessage(Client, SettingsObject.MessageOfTheDay);

            ConnectedClients.Add(Client);

            Core.SendPendingMessages();

            DatabaseLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static bool Start(WorldDataService Database)
        {
            try
            {
                InitializeCommandProcessor();
                GlobalRules.DiscoverRuleBooks(System.Reflection.Assembly.GetExecutingAssembly());
                InitializeStaticManPages();
                PersistentValueSerializer.AddGlobalSerializer(new BitArraySerializer());

                Core.Database = Database;
                Database.Initialize();

                ProscriptionList = new ProscriptionList(SettingsObject.ProscriptionListFile);

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

        public static void ProcessPlayerCommand(CommandEntry Command, PossibleMatch Match, Actor Actor)
        {
            Match.Upsert("COMMAND", Command);
            if (GlobalRules.ConsiderPerformRule("before command", Match, Actor) == PerformResult.Continue)
            {
                Command.ProceduralRules.Consider(Match, Actor);
                GlobalRules.ConsiderPerformRule("after command", Match, Actor);
            }
            GlobalRules.ConsiderPerformRule("after every command");
        }
    }

    public class BeforeAndAfterCommandRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("before command", "[Match, Actor] : Considered before every command's procedural rules are run.");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("after command", "[Match, Actor] : Considered after every command's procedural rules are run, unless the before command rules stopped the command.");

            GlobalRules.DeclarePerformRuleBook("after every command", "[] : Considered after every command, even if earlier rules stopped the command.");

            GlobalRules.DeclarePerformRuleBook<Actor>("player joined", "[Player] : Considered when a player enters the game.");

            GlobalRules.DeclarePerformRuleBook<Actor>("player left", "[Player] : Considered when a player leaves the game.");
        }
    }
}
