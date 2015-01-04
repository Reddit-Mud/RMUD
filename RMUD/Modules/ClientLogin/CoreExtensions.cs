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
        internal static ProscriptionList ProscriptionList;

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

        public static void InitializeModule_ClientLogin()
        {
            ProscriptionList = new ProscriptionList(SettingsObject.ProscriptionListFile);
        }
    }
}
