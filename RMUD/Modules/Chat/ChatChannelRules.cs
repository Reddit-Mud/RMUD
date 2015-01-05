using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Chat
{
    public class ChatChannelRules 
    {
        public static void AtStartup()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can access channel?", "[Client, Channel] : Can the client access the chat channel?");

            GlobalRules.Check<MudObject, MudObject>("can access channel?")
                .Do((client, channel) => CheckResult.Allow)
                .Name("Default allow channel access rule.");

            GlobalRules.Perform<Actor>("player joined")
                .Do(player =>
                {
                    foreach (var c in ChatChannel.ChatChannels.Where(c => c.Short == "OOC"))
                        c.Subscribers.Add(player);
                    return PerformResult.Continue;
                })
                .Name("Subscribe new players to OOC rule.");

            GlobalRules.Perform<Actor>("player left")
                .Do(player =>
                {
                    ChatChannel.RemoveFromAllChannels(player);
                    return PerformResult.Continue;
                })
                .Name("Unsubscribe players from all channels when they leave rule.");
        }
    }
}
