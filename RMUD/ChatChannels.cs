using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class ChatChannel
    {
        public String Name;
        public List<Client> Subscribers = new List<Client>();
        public Func<Client, bool> AccessFilter = null;

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
            foreach (var client in Channel.Subscribers)
            {
                if (client.IsLoggedOn)
                    client.Send(Message);
            }
        }

        public static void RemoveClientFromAllChannels(Client Client)
        {
            foreach (var channel in ChatChannels)
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Client));
        }
    }

    public class ChatChannelNameMatcher : ICommandTokenMatcher
    {
        public String ArgumentName;

        public ChatChannelNameMatcher(String ArgumentName)
        {
            this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
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
