using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Chat
{
    public class ChatChannel : MudObject
    {
        public List<Actor> Subscribers = new List<Actor>();

        public ChatChannel(String Short) : base(Short, "")
        {
            Article = "";
        }

        internal static List<ChatChannel> ChatChannels = new List<ChatChannel>();
        internal static String ChatLogsPath = "database/chatlogs/";

        internal static void RemoveFromAllChannels(MudObject Player)
        {
            foreach (var channel in ChatChannels)
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Player));
        }
    }
}
