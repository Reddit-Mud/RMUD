using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Conversation
{
    public class ConversationRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, MudObject, bool>("topic available?", "[Actor, NPC, Topic -> bool] : Is the topic available for discussion with the NPC to the actor?", "actor", "npc", "topic");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("topic response", "[Actor, NPC, Topic] : Display the response of the topic.", "actor", "npc", "topic");

            GlobalRules.Value<MudObject, MudObject, MudObject, bool>("topic available?")
                .Do((actor, npc, topic) => true)
                .Name("Topics available by default rule.");
        }

    }
}
