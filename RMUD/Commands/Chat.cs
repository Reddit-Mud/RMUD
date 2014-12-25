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
                Sequence(
                    KeyWord("SUBSCRIBE"),
                    MustMatch("I don't recognize that channel.",
                        new ChatChannelNameMatcher("CHANNEL"))),
                "Subscribe to a chat channel.")
                .Manual("Subscribes you to the specified chat channel, if you are able to access it.")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (actor.ConnectedClient == null || (channel.AccessFilter != null && !channel.AccessFilter(actor.ConnectedClient)))
                        Mud.SendMessage(actor, "You do not have access to that channel.");
                    else
                    {
                        if (!channel.Subscribers.Contains(actor.ConnectedClient))
                            channel.Subscribers.Add(actor.ConnectedClient);
                        Mud.SendMessage(actor, "You are now subscribed to " + channel.Name + ".");
                    }
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    KeyWord("UNSUBSCRIBE"),
                    MustMatch("I don't recognize that channel.",
                        new ChatChannelNameMatcher("CHANNEL"))),
                "Unubscribe from a chat channel.")
                .Manual("Unsubscribe from the specified chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
                    channel.Subscribers.RemoveAll(c => System.Object.ReferenceEquals(c, actor.ConnectedClient));
                    Mud.SendMessage(actor, "You are now unsubscribed from " + channel.Name + ".");
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                KeyWord("CHANNELS"),
                "List all available chat channels.")
                .Manual("Lists all existing chat channels.")
                .ProceduralRule((match, actor) =>
                {
                    Mud.SendMessage(actor, "~~ CHANNELS ~~");
                    foreach (var channel in Mud.ChatChannels)
                        Mud.SendMessage(actor, (channel.Subscribers.Contains(actor.ConnectedClient) ? "*" : "") + channel.Name);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    new ChatChannelNameMatcher("CHANNEL"),
                    Rest("TEXT")),
                    "Chat on a channel")
                .Name("CHAT")
                .Manual("Chat on a chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient == null) return PerformResult.Stop;
                    var channel = match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (!channel.Subscribers.Contains(actor.ConnectedClient))
                    {
                        if (channel.AccessFilter != null && !channel.AccessFilter(actor.ConnectedClient))
                        {
                            Mud.SendMessage(actor, "You do not have access to that channel.");
                            return PerformResult.Stop;
                        }

                        channel.Subscribers.Add(actor.ConnectedClient);
                        Mud.SendMessage(actor, "You are now subscribed to " + channel.Name + ".");
                    }
                    return PerformResult.Continue;
                }, "Subscribe to channels before chatting rule.")
                .ProceduralRule((match, actor) =>
                {
                    var message = match.Arguments["TEXT"].ToString();
                    var channel = match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
                    Mud.SendChatMessage(channel, "[" + channel.Name + "] " + actor.Short +
                        (message.StartsWith("\"") ?
                            (" " + message.Substring(1).Trim())
                            : (": \"" + message + "\"")));
                    return PerformResult.Continue;
                });


            Parser.AddCommand(
               new Sequence(
                   new KeyWord("RECALL", false),
                   new FailIfNoMatches(
                       new ChatChannelNameMatcher("CHANNEL"),
                       "I don't recognize that channel."),
                   new Optional(
                       new Number("COUNT"))),
                new RecallProcessor(),
                "Recall past conversation on a channel.");
        }
    }

    internal class RecallProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var channel = Match.Arguments.ValueOrDefault("CHANNEL") as ChatChannel;
            if (channel.AccessFilter != null && !channel.AccessFilter(Actor.ConnectedClient))
            {
                Mud.SendMessage(Actor, "You do not have access to that channel.");
                return;
            }

            int count = 20;
            if (Match.Arguments.ContainsKey("COUNT")) count = (Match.Arguments["COUNT"] as int?).Value;

            var logFilename = Mud.ChatLogsPath + channel.Name + ".txt";
            if (System.IO.File.Exists(logFilename))
            {
                foreach (var line in (new ReverseLineReader(logFilename)).Take(count).Reverse())
                    Mud.SendMessage(Actor, line);
            }
        }
    }
}