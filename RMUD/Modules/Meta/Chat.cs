using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Meta
{
    internal class Chat : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("SUBSCRIBE"),
                    MustMatch("I don't recognize that channel.",
                        Object("CHANNEL", new ChatChannelObjectSource()))))
                .Manual("Subscribes you to the specified chat channel, if you are able to access it.")
                .Check("can access channel?", "ACTOR", "CHANNEL")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (!channel.Subscribers.Contains(actor))
                        channel.Subscribers.Add(actor);
                    MudObject.SendMessage(actor, "You are now subscribed to <a0>.", channel);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    KeyWord("UNSUBSCRIBE"),
                    MustMatch("I don't recognize that channel.",
                        Object("CHANNEL", new ChatChannelObjectSource()))))
                .Manual("Unsubscribe from the specified chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    channel.Subscribers.RemoveAll(c => System.Object.ReferenceEquals(c, actor.ConnectedClient));
                    MudObject.SendMessage(actor, "You are now unsubscribed from <a0>.", channel);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                KeyWord("CHANNELS"))
                .Manual("Lists all existing chat channels.")
                .ProceduralRule((match, actor) =>
                {
                    MudObject.SendMessage(actor, "~~ CHANNELS ~~");
                    foreach (var channel in Core.ChatChannels)
                        MudObject.SendMessage(actor, (channel.Subscribers.Contains(actor) ? "*" : "") + channel.Short);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    Object("CHANNEL", new ChatChannelObjectSource()),
                    Rest("TEXT")))
                .Name("CHAT")
                .Manual("Chat on a chat channel.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient == null) return PerformResult.Stop;
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    if (!channel.Subscribers.Contains(actor))
                    {
                        if (GlobalRules.ConsiderCheckRule("can access channel?", actor, channel) != CheckResult.Allow)
                            return PerformResult.Stop;

                        channel.Subscribers.Add(actor);
                        MudObject.SendMessage(actor, "You are now subscribed to <a0>.", channel);
                    }
                    return PerformResult.Continue;
                }, "Subscribe to channels before chatting rule.")
                .ProceduralRule((match, actor) =>
                {
                    var message = match["TEXT"].ToString();
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    MudObject.SendChatMessage(channel, "[" + channel.Short + "] " + actor.Short +
                        (message.StartsWith("\"") ?
                            (" " + message.Substring(1).Trim())
                            : (": \"" + message + "\"")));
                    return PerformResult.Continue;
                });


            Parser.AddCommand(
                Sequence(
                    KeyWord("RECALL"),
                    MustMatch("I don't recognize that channel.",
                        Object("channel", new ChatChannelObjectSource())),
                    Optional(Number("COUNT"))))
                .Manual("Recalls past conversation on a chat channel. An optional line count parameter allows you to peek further into the past.")
                .Check("can access channel?", "ACTOR", "CHANNEL")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient == null) return PerformResult.Stop;
                    var channel = match.ValueOrDefault("CHANNEL") as ChatChannel;
                    
                    int count = 20;
                    if (match.ContainsKey("COUNT")) count = (match["COUNT"] as int?).Value;

                    var logFilename = Core.ChatLogsPath + channel.Short + ".txt";
                    if (System.IO.File.Exists(logFilename))
                        foreach (var line in (new ReverseLineReader(logFilename)).Take(count).Reverse())
                            MudObject.SendMessage(actor, line);
                    return PerformResult.Continue;
                });
        }
    }
}