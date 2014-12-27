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
                        (actor as Player).CurrentInterlocutor = match.Arguments["LOCUTOR"] as NPC;
                    return PerformResult.Continue;
                }, "Set current interlocutor rule.")
                .Perform("list topics", "ACTOR", "ACTOR");

            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("ASK"),
                        KeyWord("TELL")),
                    Optional(
                        Object("NEW-LOCUTOR", InScope, (actor, thing) =>
                        {
                            if (actor is Player && System.Object.ReferenceEquals(thing, (actor as Player).CurrentInterlocutor)) return MatchPreference.VeryLikely;
                            if (thing is NPC) return MatchPreference.Likely;
                            return MatchPreference.VeryUnlikely;
                        })),
                    OptionalKeyWord("ABOUT"),
                    FirstOf(
                        Topic("TOPIC"),
                        Rest("STRING-TOPIC"))))
                .Manual("Discusses the topic with whomever you are talking too.")
                .ProceduralRule((match, actor) =>
                {
                    if (!(actor is Player)) return PerformResult.Stop;
                    if (match.Arguments.ContainsKey("NEW-LOCUTOR"))
                    {
                        var newLocutor = match.Arguments["NEW-LOCUTOR"] as MudObject;
                        if (GlobalRules.ConsiderCheckRule("can converse?", newLocutor, actor, newLocutor) == CheckResult.Disallow) return PerformResult.Stop;
                        if (!System.Object.ReferenceEquals(newLocutor, (actor as Player).CurrentInterlocutor))
                        {
                            GlobalRules.ConsiderPerformRule("greet", newLocutor, actor, newLocutor);
                            (actor as Player).CurrentInterlocutor = newLocutor as NPC;
                        }
                    }
                    match.Arguments.Upsert("LOCUTOR", (actor as Player).CurrentInterlocutor);
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
                .ProceduralRule((match, actor) =>
                {
                    if (!match.Arguments.ContainsKey("TOPIC"))
                    {
                        if ((actor as Player).CurrentInterlocutor.DefaultResponse != null)
                            match.Arguments.Upsert("TOPIC", (actor as Player).CurrentInterlocutor.DefaultResponse);
                        else
                        {
                            Mud.SendMessage(actor, "That doesn't seem to be a topic I understand.");
                            return PerformResult.Stop;
                        }
                    }
                    return PerformResult.Continue;
                }, "Supply default topic if needed rule.")
                .Perform("converse", "LOCUTOR", "ACTOR", "LOCUTOR", "TOPIC")
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
                    var suggestedTopics = npc.ConversationTopics.Where(topic => topic.IsAvailable((actor as Player), npc) && !Conversation.HasKnowledgeOfTopic((actor as Player), npc, topic.ID)).Take(4);
                    if (suggestedTopics.Count() != 0)
                        Mud.SendMessage(actor, "Suggested topics: " + String.Join(", ", suggestedTopics.Select(topic => topic.Topic)) + ".");
                    return PerformResult.Continue;
                })
                .Name("List un-discussed available topics rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, ConversationTopic>("converse", "[Actor, NPC, Topic] : Handle the actor discussing the topic with the npc.");

            GlobalRules.Perform<MudObject, MudObject, ConversationTopic>("converse")
                .Do((actor, npc, topic) =>
                {
                    Mud.SendMessage(actor, String.Format("You discuss '{0}' with <the0>.", topic.Topic), npc);
                    Mud.SendExternalMessage(actor, String.Format("^<the0> discusses '{0}' with <the1>.", topic.Topic), actor, npc);

                    if (topic.ResponseType == ConversationTopic.ResponseTypes.Normal)
                    {
                        var response = topic.NormalResponse.Expand(actor as Actor, npc);
                        Mud.SendLocaleMessage(actor, response, npc);
                    }
                    else if (topic.ResponseType == ConversationTopic.ResponseTypes.Silent)
                    {
                        topic.SilentResponse(actor as Player, npc as NPC, topic);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    foreach (var player in Mud.EnumerateObjectTree(Mud.FindLocale(actor)).Where(o => o is Player).Select(o => o as Player))
                        Conversation.GrantKnowledgeOfTopic(player, npc as NPC, topic.ID);

                    return PerformResult.Continue;
                })
                .Name("Expand and discuss the topic rule.");
        }
    }

}
