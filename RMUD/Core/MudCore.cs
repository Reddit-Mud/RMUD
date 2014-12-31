using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        internal static List<Client> ConnectedClients = new List<Client>();
        internal static Mutex DatabaseLock = new Mutex();
        private static bool ShuttingDown = false;

        internal static Settings SettingsObject;
        internal static ProscriptionList ProscriptionList;

        public static void ClientDisconnected(Client client)
        {
            DatabaseLock.WaitOne();
            RemoveClientFromAllChannels(client);
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

        public enum ClientAcceptanceStatus
        {
            Accepted,
            Rejected,
        }

        public static ClientAcceptanceStatus ClientConnected(Client Client)
        {
            var ban = ProscriptionList.IsBanned(Client.IPString);
            if (ban.Banned)
            {
                LogError("Rejected connection from " + Client.IPString + ". Matched ban " + ban.SourceBan.Glob + " Reason: " + ban.SourceBan.Reason);
                return ClientAcceptanceStatus.Rejected;
            }

            DatabaseLock.WaitOne();

            Client.CommandHandler = LoginCommandHandler;

            MudObject.SendMessage(Client, MudObject.SettingsObject.Banner);
            MudObject.SendMessage(Client, MudObject.SettingsObject.MessageOfTheDay);

            ConnectedClients.Add(Client);

            SendPendingMessages();

            DatabaseLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static bool Start(String basePath)
        {
            try
            {
                PrepareSerializers();
                InitializeDatabase(basePath);

                SettingsObject = new Settings();
                var settings = GetObject("settings") as Settings;
                if (settings == null) LogError("No settings object found in database. Using default settings.");
                else SettingsObject = settings;
                NamedObjects.Clear();

                ProscriptionList = new ProscriptionList(basePath + SettingsObject.ProscriptionList);

                InitializeCommandProcessor();
                GlobalRules.DiscoverRuleBooks(System.Reflection.Assembly.GetExecutingAssembly());
                InitializeStaticManPages();

                    var start = DateTime.Now;
                    var errorReported = false;
                    InitialBulkCompile((s) => {
                        LogError(s);
                        errorReported = true;
                    });

                    if (errorReported) Console.WriteLine("Bulk compilation failed. Using ad-hoc compilation as fallback.");
                    else 
                        Console.WriteLine("Total compilation in {0}.", DateTime.Now - start);

                Console.WriteLine("Engine ready with path " + basePath + ".");
            }
            catch (Exception e)
            {
                LogError("Failed to start mud engine.");
                LogError(e.Message);
                LogError(e.StackTrace);
                throw e;
            }
            return true;
        }

        public static void Shutdown()
        {
            ShuttingDown = true;
        }

        public static void ProcessPlayerCommand(CommandEntry Command, PossibleMatch Match, Actor Actor)
        {
            Command.ProceduralRules.Consider(Match, Actor);
            if (Actor != null)
                CheckQuestStatus(Actor);
        }
    }
}
