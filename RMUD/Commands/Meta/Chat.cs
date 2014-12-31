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
                        new ChatChannelNameMatcher("CHANNEL"))))
                .Manual("Subscribes you to the specified chat channel, if you are able to access it.")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (actor.ConnectedClient == null || (channel.AccessFilter != null && !channel.AccessFilter(actor.ConnectedClient)))
                        MudObject.SendMessage(actor, "You do not have access to that channel.");
                    else
                    {
                        if (!channel.Subscribers.Contains(actor.ConnectedClient))
                            channel.Subscribers.Add(actor.ConnectedClient);
                        MudObject.SendMessage(actor, "You are now subscribed to " + channel.Name + ".");
                    }
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    KeyWord("UNSUBSCRIBE"),
                    MustMatch("I don't recognize that channel.",
                        new ChatChannelNameMatcher("CHANNEL"))))
                .Manual("Unsubscribe from the specified chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    channel.Subscribers.RemoveAll(c => System.Object.ReferenceEquals(c, actor.ConnectedClient));
                    MudObject.SendMessage(actor, "You are now unsubscribed from " + channel.Name + ".");
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                KeyWord("CHANNELS"))
                .Manual("Lists all existing chat channels.")
                .ProceduralRule((match, actor) =>
                {
                    MudObject.SendMessage(actor, "~~ CHANNELS ~~");
                    foreach (var channel in MudObject.ChatChannels)
                        MudObject.SendMessage(actor, (channel.Subscribers.Contains(actor.ConnectedClient) ? "*" : "") + channel.Name);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    new ChatChannelNameMatcher("CHANNEL"),
                    Rest("TEXT")))
                .Name("CHAT")
                .Manual("Chat on a chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient == null) return PerformResult.Stop;
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (!channel.Subscribers.Contains(actor.ConnectedClient))
                    {
                        if (channel.AccessFilter != null && !channel.AccessFilter(actor.ConnectedClient))
                        {
                            MudObject.SendMessage(actor, "You do not have access to that channel.");
                            return PerformResult.Stop;
                        }

                        channel.Subscribers.Add(actor.ConnectedClient);
                        MudObject.SendMessage(actor, "You are now subscribed to " + channel.Name + ".");
                    }
                    return PerformResult.Continue;
                }, "Subscribe to channels before chatting rule.")
                .ProceduralRule((match, actor) =>
                {
                    var message = match["TEXT"].ToString();
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    MudObject.SendChatMessage(channel, "[" + channel.Name + "] " + actor.Short +
                        (message.StartsWith("\"") ?
                            (" " + message.Substring(1).Trim())
                            : (": \"" + message + "\"")));
                    return PerformResult.Continue;
                });


            Parser.AddCommand(
                Sequence(
                    KeyWord("RECALL"),
                    MustMatch("I don't recognize that channel.",
                        new ChatChannelNameMatcher("CHANNEL")),
                    Optional(Number("COUNT"))))
                .Manual("Recalls past conversation on a chat channel. An optional line count parameter allows you to peek further into the past.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient == null) return PerformResult.Stop;
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (channel.AccessFilter != null && !channel.AccessFilter(actor.ConnectedClient))
                    {
                        MudObject.SendMessage(actor, "You do not have access to that channel.");
                        return PerformResult.Stop;
                    }

                    int count = 20;
                    if (match.ContainsKey("COUNT")) count = (match["COUNT"] as int?).Value;

                    var logFilename = MudObject.ChatLogsPath + channel.Name + ".txt";
                    if (System.IO.File.Exists(logFilename))
                        foreach (var line in (new ReverseLineReader(logFilename)).Take(count).Reverse())
                            MudObject.SendMessage(actor, line);
                    return PerformResult.Continue;
                });
        }
    }
}