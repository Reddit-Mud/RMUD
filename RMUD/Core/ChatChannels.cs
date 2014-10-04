using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class ChatMessage
    {
        public DateTime Time;
        public String Message;
    }

    public class ChatChannel
    {
        public String Name;
        public List<Client> Subscribers = new List<Client>();
        public Func<Client, bool> AccessFilter = null;
        public List<ChatMessage> ChatHistory = new List<ChatMessage>();

        public ChatChannel(String Name, Func<Client, bool> AccessFilter = null)
        {
            this.Name = Name;
            this.AccessFilter = AccessFilter;
        }
    }

    public static partial class Mud
    {
        public static List<ChatChannel> ChatChannels = new List<ChatChannel>();

        public static ChatChannel FindChatChannel(String Name)
        {
            return ChatChannels.FirstOrDefault(c => c.Name == Name);
        }

        public static void SendChatMessage(ChatChannel Channel, String Message)
        {
            Channel.ChatHistory.Add(new ChatMessage { Time = DateTime.Now, Message = Message });
            if (Channel.ChatHistory.Count > Mud.SettingsObject.MaximumChatChannelLogSize)
                Channel.ChatHistory.RemoveAt(0);

            foreach (var client in Channel.Subscribers)
            {
                if (client.IsLoggedOn)
                    Mud.SendMessage(client, Message);
            }
        }

        public static void RemoveClientFromAllChannels(Client Client)
        {
            foreach (var channel in ChatChannels)
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Client));
        }
    }

    public class ChatChannelNameMatcher : CommandTokenMatcher
    {
        public String ArgumentName;

        public ChatChannelNameMatcher(String ArgumentName)
        {
            this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
            if (State.Next == null) return r;
            var channel = Mud.FindChatChannel(State.Next.Value.ToUpper());
            if (channel != null)
            {
                var possibleMatch = new PossibleMatch(State.Arguments, State.Next.Next);
                possibleMatch.Arguments.Upsert(ArgumentName, channel);
                r.Add(possibleMatch);
            }
            return r;
        }

        public string Emit()
        {
            return "[CHANNEL]";
        }
    }
}
