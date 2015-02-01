using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space
{
    internal static class Game
    {
        internal static bool BlockingConversation = false;
        internal static bool SuppressTopics = false;

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<RMUD.PossibleMatch, RMUD.Actor>("before command")
                .When((m, a) => BlockingConversation == true)
                .Do((match, actor) =>
                {
                    var command = match["COMMAND"] as RMUD.CommandEntry;
                    if (command.IsNamed("ASK") || command.IsNamed("HELP") || command.IsNamed("TOPICS"))
                        return RMUD.PerformResult.Continue;
                    RMUD.MudObject.SendMessage(actor, "Sal, I really need to talk about this.");
                    RMUD.Core.EnqueuActorCommand(actor, "TOPICS");
                    return RMUD.PerformResult.Stop;
                })
                .Name("Can only converse during a blocking conversation rule.");

            GlobalRules.Perform<Player>("list topics")
                .When(player => SuppressTopics)
                .Do(player => RMUD.PerformResult.Stop);

            GlobalRules.Perform<Player>("list topics")
                .Do(player =>
                {
                    var npc = RMUD.MudObject.GetObject("Dan");
                    var availableTopics = npc.GetPropertyOrDefault<List<RMUD.MudObject>>("conversation-topics", new List<RMUD.MudObject>()).Where(topic => GlobalRules.ConsiderValueRule<bool>("topic available?", player, npc, topic));

                    if (availableTopics.Count() == 0)
                        BlockingConversation = false;

                    return RMUD.PerformResult.Continue;
                })
                .Name("Unblock game if no available topics rule.");

            GlobalRules.Perform<Player>("player joined")
                .First
                .Do((actor) =>
                {
                    BlockingConversation = true;

                    RMUD.MudObject.SendMessage(actor, "Sal? Sal? Can you hear me?");
                    actor.SetProperty("interlocutor", RMUD.MudObject.GetObject("Dan"));
                    RMUD.Core.EnqueuActorCommand(actor, "topics");
                    return RMUD.PerformResult.Stop;
                });
        }
    }
}
