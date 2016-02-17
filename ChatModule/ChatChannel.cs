using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ChatModule
{
    public class ChatChannel : MudObject
    {
        public List<Actor> Subscribers = new List<Actor>();

        public ChatChannel(String Short) : base(Short, "")
        {
            SetProperty("Article", "");
        }

        internal static List<ChatChannel> ChatChannels = new List<ChatChannel>();
        internal static String ChatLogsPath = "database/chatlogs/";

        internal static void RemoveFromAllChannels(MudObject Player)
        {
            foreach (var channel in ChatChannels)
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Player));
        }

        public static void SendChatMessage(ChatChannel Channel, String Message)
        {
            var realMessage = String.Format("{0} : {1}", DateTime.Now, Message);

            var chatLogFilename = ChatChannel.ChatLogsPath + Channel.GetProperty<String>("Short") + ".txt";
            System.IO.Directory.CreateDirectory(ChatChannel.ChatLogsPath);
            System.IO.File.AppendAllText(chatLogFilename, realMessage + "\n");

            foreach (var client in Channel.Subscribers.Where(c => c.ConnectedClient != null))
                MudObject.SendMessage(client, realMessage);
        }
    }
}
