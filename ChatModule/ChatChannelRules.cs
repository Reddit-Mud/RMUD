using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ChatModule
{
    public class ChatChannelRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can access channel?", "[Client, Channel] : Can the client access the chat channel?", "actor", "channel");

            GlobalRules.Check<MudObject, MudObject>("can access channel?")
                .Do((client, channel) => SharpRuleEngine.CheckResult.Allow)
                .Name("Default allow channel access rule.");

            GlobalRules.Perform<MudObject>("player joined")
                .Do(player =>
                {
                    foreach (var c in ChatChannel.ChatChannels.Where(c => c.GetProperty<String>("short") == "OOC"))
                        c.Subscribers.Add(player);
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Subscribe new players to OOC rule.");

            GlobalRules.Perform<MudObject>("player left")
                .Do(player =>
                {
                    ChatChannel.RemoveFromAllChannels(player);
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Unsubscribe players from all channels when they leave rule.");


            ChatChannel.ChatChannels.Clear();
            ChatChannel.ChatChannels.Add(new ChatChannel("OOC"));

            var senate = new ChatChannel("SENATE");
            senate.Check<MudObject, MudObject>("can access channel?")
                .When((actor, channel) => actor.GetPropertyOrDefault<int>("rank") < 100)
                .Do((actor, channel) =>
                {
                    MudObject.SendMessage(actor, "You must have a rank of 100 or greater to access this channel.");
                    return SharpRuleEngine.CheckResult.Disallow;
                });
            ChatChannel.ChatChannels.Add(senate);
        }
    }
}
