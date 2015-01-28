using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ConversationModule
{
    public class ConversationRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can converse?", "[Actor, Item] : Can the actor converse with the item?", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .When((actor, item) => !(item is NPC))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "You can't converse with that.");
                    return CheckResult.Disallow;
                })
                .Name("Can only converse with NPCs rule.");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Locutor must be visible rule.");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .Last
                .Do((actor, item) => CheckResult.Allow)
                .Name("Let them chat rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("greet", "[Actor, NPC] : Handle an actor greeting an NPC.", "actor", "npc");

            GlobalRules.DeclarePerformRuleBook<MudObject>("list topics", "[Actor] : List conversation topics available to the actor.", "actor");

            GlobalRules.Perform<MudObject>("list topics")
                .When(actor => !(actor is Player) || actor.GetProperty<NPC>("interlocutor") == null)
                .Do(actor =>
                {
                    MudObject.SendMessage(actor, "You aren't talking to anyone.");
                    return PerformResult.Stop;
                })
                .Name("Need interlocutor to list topics rule.");

            GlobalRules.Perform<MudObject>("list topics")
                .Do(actor =>
                {
                    if (!(actor is Player)) return PerformResult.Stop;
                    var npc = actor.GetProperty<NPC>("interlocutor");
                    var suggestedTopics = npc.GetPropertyOrDefault<List<MudObject>>("conversation-topics", new List<MudObject>()).Where(topic => GlobalRules.ConsiderValueRule<bool>("topic available?", actor, npc, topic));

                    if (suggestedTopics.Count() != 0)
                        MudObject.SendMessage(actor, "Suggested topics: " + String.Join(", ", suggestedTopics.Select(topic => topic.Short)) + ".");

                    return PerformResult.Continue;
                })
                .Name("List un-discussed available topics rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("discuss topic", "[Actor, NPC, Topic] : Handle the actor discussing the topic with the npc.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("discuss topic")
                .Do((actor, npc, topic) =>
                {
                    GlobalRules.ConsiderPerformRule("topic response", actor, npc, topic);
                    return PerformResult.Continue;
                })
                .Name("Show topic response when discussing topic rule.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("topic response")
                .Do((actor, npc, topic) =>
                {
                    MudObject.SendMessage(actor, "There doesn't seem to be a response defined for that topic.");
                    return PerformResult.Stop;
                })
                .Name("No response rule for the topic rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, MudObject, bool>("topic available?", "[Actor, NPC, Topic -> bool] : Is the topic available for discussion with the NPC to the actor?", "actor", "npc", "topic");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("topic response", "[Actor, NPC, Topic] : Display the response of the topic.", "actor", "npc", "topic");

            GlobalRules.Value<MudObject, MudObject, MudObject, bool>("topic available?")
                .Do((actor, npc, topic) => true)
                .Name("Topics available by default rule.");
        }

    }
}
