using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Chat : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("SUBSCRIBE", false),
                    new FirstOf(
                        new ChatChannelNameMatcher("CHANNEL"),
                        new Rest("GARBAGE"))),
                new SubscribeProcessor(),
                "Subscribe to a chat channel.");

            Parser.AddCommand(
                new Sequence(
                    new KeyWord("UNSUBSCRIBE", false),
                    new FirstOf(
                        new ChatChannelNameMatcher("CHANNEL"),
                        new Rest("GARBAGE"))),
                new UnsubscribeProcessor(),
                "Unubscribe from a chat channel.");

            Parser.AddCommand(
                new KeyWord("CHANNELS", false),
                new ListChannelsProcessor(),
                "List all available chat channels.");

            Parser.AddCommand(
                new Sequence(
                    new ChatChannelNameMatcher("CHANNEL"),
                    new Rest("TEXT")),
                new ChatProcessor(),
                "Chat on a channel.");
        }
	}

	internal class SubscribeProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;

            var channel = Match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
            if (channel == null)
                    Actor.ConnectedClient.Send("I don't recognize that channel.\r\n");
            else
            {
                if (!channel.Subscribers.Contains(Actor.ConnectedClient))
                    channel.Subscribers.Add(Actor.ConnectedClient);
                Actor.ConnectedClient.Send("You are now subscribed to " + channel.Name + ".\r\n");
            }
		}
	}

    internal class UnsubscribeProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var channel = Match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
            if (channel == null)
                Actor.ConnectedClient.Send("I don't recognize that channel.\r\n");
            else
            {
                channel.Subscribers.RemoveAll(c => Object.ReferenceEquals(c, Actor.ConnectedClient));
                Actor.ConnectedClient.Send("You are now unsubscribed from " + channel.Name + ".\r\n");
            }
        }
    }

    internal class ListChannelsProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var builder = new StringBuilder();
            builder.Append("~~~ CHAT CHANNELS ~~~\r\n");
            foreach (var channel in Mud.ChatChannels)
            {
                if (channel.Subscribers.Contains(Actor.ConnectedClient))
                    builder.Append("*");
                builder.Append(String.Format("{0}\r\n", channel.Name));
            }
            builder.Append("\r\n");

            Actor.ConnectedClient.Send(builder.ToString());
        }
    }

    internal class ChatProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var channel = Match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
            if (channel == null) //Shouldn't have gotten here is a channel name wasn't matched.
                Actor.ConnectedClient.Send("I don't recognize that channel.\r\n");
            else
            {
                if (!channel.Subscribers.Contains(Actor.ConnectedClient))
                {
                    channel.Subscribers.Add(Actor.ConnectedClient);
                    Actor.ConnectedClient.Send("You are now subscribed to " + channel.Name + ".\r\n");
                }

                var messageBuilder = new StringBuilder();
                messageBuilder.Append(String.Format("[{0}] {1} \"", channel.Name, Actor.Short));
                Mud.AssembleText(Match.Arguments["TEXT"] as LinkedListNode<String>, messageBuilder);
                messageBuilder.Append("\"\r\n");

                Mud.SendChatMessage(channel, messageBuilder.ToString());
            }
        }
    }
}
