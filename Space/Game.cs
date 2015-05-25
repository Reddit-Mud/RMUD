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
        internal static bool SuitRepaired = false;

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<RMUD.PossibleMatch, RMUD.Actor>("before command")
                .When((m, a) => BlockingConversation == true)
                .Do((match, actor) =>
                {
                    var command = match["COMMAND"] as RMUD.CommandEntry;
                    if (command.IsNamed("ASK") || command.IsNamed("HELP") || command.IsNamed("TOPICS"))
                        return SharpRuleEngine.PerformResult.Continue;
                    RMUD.MudObject.SendMessage(actor, "Sal, I really need to talk about this.");
                    RMUD.Core.EnqueuActorCommand(actor, "TOPICS");
                    return SharpRuleEngine.PerformResult.Stop;
                })
                .Name("Can only converse during a blocking conversation rule.");

            GlobalRules.Perform<Player, RMUD.MudObject, ConversationModule.Topic>("discuss topic")
                .Do((actor, npc, topic) =>
                {
                    topic.SetProperty("discussed", true);
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Mark topic discussed rule.");

            GlobalRules.Perform<Player>("list topics")
                .When(player => SuppressTopics)
                .Do(player => SharpRuleEngine.PerformResult.Stop);

            GlobalRules.Perform<Player>("list topics")
                .Do(player =>
                {
                    var npc = player.GetProperty<RMUD.NPC>("interlocutor");
                    var availableTopics = npc.GetPropertyOrDefault<List<RMUD.MudObject>>("conversation-topics", new List<RMUD.MudObject>()).Where(topic => GlobalRules.ConsiderValueRule<bool>("topic available?", player, npc, topic));

                    if (availableTopics.Count() == 0)
                        BlockingConversation = false;

                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Unblock game if no available topics rule.");

            GlobalRules.Perform<Player>("singleplayer game started")
                .First
                .Do((actor) =>
                {
                    //BlockingConversation = true;

                    //RMUD.MudObject.SendMessage(actor, "Sal? Sal? Can you hear me?");
                    //actor.SetProperty("interlocutor", RMUD.MudObject.GetObject("DanConversation0"));
                    //RMUD.Core.EnqueuActorCommand(actor, "topics");
                    
                    RMUD.MudObject.Move(actor, RMUD.MudObject.GetObject("Start"));
                    RMUD.Core.EnqueuActorCommand(actor, "look");
                    return SharpRuleEngine.PerformResult.Stop;
                });
        }
    }
}
