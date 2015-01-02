using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD.Modules.Chat;

namespace RMUD
{
    public partial class MudObject
    {
        public static void SendChatMessage(ChatChannel Channel, String Message)
        {
            var realMessage = String.Format("{0} : {1}", DateTime.Now, Message);

            var chatLogFilename = ChatChannel.ChatLogsPath + Channel.Short + ".txt";
            System.IO.Directory.CreateDirectory(ChatChannel.ChatLogsPath);
            System.IO.File.AppendAllText(chatLogFilename, realMessage + "\n");

            foreach (var client in Channel.Subscribers.Where(c => c.ConnectedClient != null))
                MudObject.SendMessage(client, realMessage);
        }
    }
}
