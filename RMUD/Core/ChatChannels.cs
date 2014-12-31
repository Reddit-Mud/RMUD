using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class ChatChannelRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can access channel?", "[Client, Channel] : Can the client access the chat channel?");

            GlobalRules.Check<MudObject, MudObject>("can access channel?")
                .Do((client, channel) => CheckResult.Allow)
                .Name("Default allow channel access rule.");
        }
    }

    public class ChatChannel : MudObject
    {
        public List<Actor> Subscribers = new List<Actor>();

        public ChatChannel(String Short) : base(Short, "")
        {
            Article = "";
        }
    }

    public static partial class Core
    {
        internal static List<ChatChannel> ChatChannels = new List<ChatChannel>();

        internal static void RemoveClientFromAllChannels(Client Client)
        {
            foreach (var channel in ChatChannels)
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Client));
        }
    }

    public partial class MudObject
    {
        public static void SendChatMessage(ChatChannel Channel, String Message)
        {
            var realMessage = String.Format("{0} : {1}", DateTime.Now, Message);

            var chatLogFilename = Core.ChatLogsPath + Channel.Short + ".txt";
            System.IO.Directory.CreateDirectory(Core.ChatLogsPath);
            System.IO.File.AppendAllText(chatLogFilename, realMessage + "\n");

            foreach (var client in Channel.Subscribers.Where(c => c.ConnectedClient != null))
                MudObject.SendMessage(client, realMessage);
        }
    }

    public class ChatChannelObjectSource : IObjectSource
    {
        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            return new List<MudObject>(Core.ChatChannels);
        }
    }
}
