using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using RMUD;

namespace NetworkModule
{
    internal enum ClientAcceptanceStatus
    {
        Accepted,
        Rejected,
    }

    public static class Clients
    {
        public static List<Client> ConnectedClients = new List<Client>();
        private static Mutex ClientLock = new Mutex();
        internal static ProscriptionList ProscriptionList;

        internal static void ClientDisconnected(Client client)
        {
            ClientLock.WaitOne();
            ConnectedClients.Remove(client);
            client.Player.GetProperty<Account>("account").LoggedInCharacter = null;
            Core.RemovePlayer(client.Player);
            ClientLock.ReleaseMutex();
        }

        internal static ClientAcceptanceStatus ClientConnected(NetworkClient Client)
        {
            var ban = ProscriptionList.IsBanned(Client.IPString);
            if (ban.Banned)
            {
                Core.LogError("Rejected connection from " + Client.IPString + ". Matched ban " + ban.SourceBan.Glob + " Reason: " + ban.SourceBan.Reason);
                return ClientAcceptanceStatus.Rejected;
            }

            ClientLock.WaitOne();

            var Player = new Actor();
            Player.CommandHandler = new LoginCommandHandler();
            Core.TiePlayerToClient(Client, Player);

            MudObject.SendMessage(Client, Core.SettingsObject.Banner);
            MudObject.SendMessage(Client, Core.SettingsObject.MessageOfTheDay);

            ConnectedClients.Add(Client);

            Core.SendPendingMessages();

            ClientLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            ProscriptionList = new ProscriptionList("proscriptions.txt");
        }

        public static void SendGlobalMessage(String Message, params MudObject[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;

            foreach (var client in ConnectedClients)
                RMUD.MudObject.SendMessage(client, Message, MentionedObjects);
        }
    }
}
