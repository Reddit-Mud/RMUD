using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class ConversationCommandFactory : CommandFactory, DeclaresRules
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("GREET"),
                        KeyWord("HELLO")),
                    MustMatch("Whom do you want to greet?",
                        Object("LOCUTOR", InScope, (actor, thing) =>
                        {
                            if (thing is NPC) return MatchPreference.VeryLikely;
                            else return MatchPreference.VeryUnlikely;
                        }))))
                .Manual("Initiates a conversation with the npc.")
                .Check("can converse?", "LOCUTOR", "ACTOR", "LOCUTOR")
                .Perform("greet", "LOCUTOR", "ACTOR", "LOCUTOR")
                .ProceduralRule((match, actor) =>
                {
                    if (actor is Player)
                        (actor as Player).CurrentInterlocutor = match["LOCUTOR"] as NPC;
                    return PerformResult.Continue;
                }, "Set current interlocutor rule.")
                .Perform("list topics", "ACTOR", "ACTOR");

            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("ASK"),
                        KeyWord("TELL")),
                        FirstOf(
                            Object("TOPIC", new TopicSource()),
                            Sequence(
                                Object("NEW-LOCUTOR", InScope, (actor, thing) =>
                                {
                                    if (actor is Player && System.Object.ReferenceEquals(thing, (actor as Player).CurrentInterlocutor)) return MatchPreference.VeryLikely;
                                    if (thing is NPC) return MatchPreference.Likely;
                                    return MatchPreference.VeryUnlikely;
                                }),
                                OptionalKeyWord("ABOUT"),
                                FirstOf(
                                    Object("TOPIC", new TopicSource("NEW-LOCUTOR")),
                                    Rest("STRING-TOPIC"))),
                            Rest("STRING-TOPIC"))))
                .Manual("Discusses the topic with whomever you are talking too.")
                .ProceduralRule((match, actor) =>
                {
                    if (!(actor is Player)) return PerformResult.Stop;
                    if (match.ContainsKey("NEW-LOCUTOR"))
                    {
                        var newLocutor = match["NEW-LOCUTOR"] as MudObject;
                        if (GlobalRules.ConsiderCheckRule("can converse?", newLocutor, actor, newLocutor) == CheckResult.Disallow) return PerformResult.Stop;
                        if (!System.Object.ReferenceEquals(newLocutor, (actor as Player).CurrentInterlocutor))
                        {
                            GlobalRules.ConsiderPerformRule("greet", newLocutor, actor, newLocutor);
                            (actor as Player).CurrentInterlocutor = newLocutor as NPC;
                        }
                    }
                    match.Upsert("LOCUTOR", (actor as Player).CurrentInterlocutor);
                    return PerformResult.Continue;
                }, "Implicitly greet new locutors rule.")
                .ProceduralRule((match, actor) =>
                {
                    if ((actor as Player).CurrentInterlocutor == null)
                    {
                        Mud.SendMessage(actor, "You aren't talking to anyone.");
                        return PerformResult.Stop;
                    }
                    return PerformResult.Continue;
                }, "Must be talking to someone rule.")
                .Check("can converse?", "LOCUTOR", "ACTOR", "LOCUTOR")
                .Perform("discuss topic", "LOCUTOR", "ACTOR", "LOCUTOR", "TOPIC")
                .Perform("list topics", "ACTOR", "ACTOR");

            Parser.AddCommand(
                KeyWord("TOPICS"))
                .Manual("Lists topics currently available.")
                .Perform("list topics", "ACTOR", "ACTOR");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can converse?", "[Actor, Item] : Can the actor converse with the item?");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .When((actor, item) => !(item is NPC))
                .Do((actor, item) =>
                {
                    Mud.SendMessage(actor, "You can't converse with that.");
                    return CheckResult.Disallow;
                })
                .Name("Can only converse with NPCs rule.");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .Do((actor, item) => GlobalRules.IsVisibleTo(actor, item))
                .Name("Locutor must be visible rule.");

            GlobalRules.Check<MudObject, MudObject>("can converse?")
                .Last
                .Do((actor, item) => CheckResult.Allow)
                .Name("Let them chat rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("greet", "[Actor, NPC] : Handle an actor greeting an NPC.");

            GlobalRules.DeclarePerformRuleBook<MudObject>("list topics", "[Actor] : List conversation topics available to the actor.");

            GlobalRules.Perform<MudObject>("list topics")
                .When(actor => !(actor is Player) || ((actor as Player).CurrentInterlocutor == null))
                .Do(actor =>
                {
                    Mud.SendMessage(actor, "You aren't talking to anyone.");
                    return PerformResult.Stop;
                })
                .Name("Need interlocutor to list topics rule.");

            GlobalRules.Perform<MudObject>("list topics")
                .Do(actor =>
                {
                    if (!(actor is Player)) return PerformResult.Stop;
                    var npc = (actor as Player).CurrentInterlocutor;
                    var suggestedTopics = npc.ConversationTopics.Where(topic => GlobalRules.ConsiderValueRule<bool>("topic available?", topic, actor, npc, topic));
                    if (suggestedTopics.Count() != 0)
                        Mud.SendMessage(actor, "Suggested topics: " + String.Join(", ", suggestedTopics.Select(topic => topic.Short)) + ".");
                    return PerformResult.Continue;
                })
                .Name("List un-discussed available topics rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("discuss topic", "[Actor, NPC, Topic] : Handle the actor discussing the topic with the npc.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("discuss topic")
                .Do((actor, npc, topic) =>
                {
                    GlobalRules.ConsiderPerformRule("topic response", topic, actor, npc, topic);
                    return PerformResult.Continue;
                })
                .Name("Show topic response when discussing topic rule.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("topic response")
                .Do((actor, npc, topic) =>
                {
                    Mud.SendMessage(actor, "There doesn't seem to be a response defined for that topic.");
                    return PerformResult.Stop;
                })
                .Name("No response rule for the topic rule.");

        }
    }

}
