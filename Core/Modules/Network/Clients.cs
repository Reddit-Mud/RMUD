using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD.Modules.Network
{
    internal enum ClientAcceptanceStatus
    {
        Accepted,
        Rejected,
    }

    public static class Clients
    {
        internal static List<Client> ConnectedClients = new List<Client>();
        internal static ProscriptionList ProscriptionList;

        internal static void ClientDisconnected(Client client)
        {
            Core.DatabaseLock.WaitOne();
            ConnectedClients.Remove(client);
            Core.RemovePlayer(client.Player);
            client.Account.LoggedInCharacter = null;
            Core.DatabaseLock.ReleaseMutex();
        }

        internal static ClientAcceptanceStatus ClientConnected(Modules.Network.NetworkClient Client)
        {
            var ban = ProscriptionList.IsBanned(Client.IPString);
            if (ban.Banned)
            {
                Core.LogError("Rejected connection from " + Client.IPString + ". Matched ban " + ban.SourceBan.Glob + " Reason: " + ban.SourceBan.Reason);
                return ClientAcceptanceStatus.Rejected;
            }

            Core.DatabaseLock.WaitOne();

            var Player = new Actor();
            Player.CommandHandler = new Modules.Network.LoginCommandHandler();
            Core.TiePlayerToClient(Client, Player);

            MudObject.SendMessage(Client, Core.SettingsObject.Banner);
            MudObject.SendMessage(Client, Core.SettingsObject.MessageOfTheDay);

            ConnectedClients.Add(Client);

            Core.SendPendingMessages();

            Core.DatabaseLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            ProscriptionList = new ProscriptionList("proscriptions.txt");
        }
    }
}
