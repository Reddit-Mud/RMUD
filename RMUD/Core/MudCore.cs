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
            Core.RemoveClientFromAllChannels(client);
            ConnectedClients.Remove(client);
            if (client.Player != null)
            {
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

            Client.CommandHandler = LoginCommandHandler;

            MudObject.SendMessage(Client, SettingsObject.Banner);
            MudObject.SendMessage(Client, SettingsObject.MessageOfTheDay);

            ConnectedClients.Add(Client);

            Core.SendPendingMessages();

            DatabaseLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static bool Start(String basePath)
        {
            ChatLogsPath = basePath + "chatlogs/";
            AccountsPath = basePath + "accounts/";

            try
            {
                InitializeCommandProcessor();
                GlobalRules.DiscoverRuleBooks(System.Reflection.Assembly.GetExecutingAssembly());
                InitializeStaticManPages();
                AddGlobalSerializer(new BitArraySerializer());

                Database = new GithubDatabase();
                Database.Initialize(basePath);

                ProscriptionList = new ProscriptionList(basePath + SettingsObject.ProscriptionListFile);

                StartCommandProcesor();
                Console.WriteLine("Engine ready with path " + basePath + ".");
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
        }
    }

    public class BeforeAndAfterCommandRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("before command", "[Match, Actor] : Considered before every command's procedural rules are run.");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("after command", "[Match, Actor] : Considered after every command's procedural rules are run, unless the before command rules stopped the command.");
        }
    }
}
