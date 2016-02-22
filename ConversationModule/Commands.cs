using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace ConversationModule
{
    internal class ConversationCommandFactory : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("GREET"),
                        KeyWord("HELLO")),
                    MustMatch("@convo greet whom",
                        Object("LOCUTOR", InScope, (actor, thing) =>
                        {
                            if (thing.GetPropertyOrDefault<bool>("actor?")) return MatchPreference.VeryLikely;
                            else return MatchPreference.VeryUnlikely;
                        }))))
                .ID("Conversation:Greet")
                .Manual("Initiates a conversation with the npc.")
                .Check("can converse?", "ACTOR", "LOCUTOR")
                .BeforeActing()
                .Perform("greet", "ACTOR", "LOCUTOR")
                .AfterActing()
                .ProceduralRule((match, actor) =>
                {
                    actor.SetProperty("interlocutor", match["LOCUTOR"] as MudObject);
                    return PerformResult.Continue;
                }, "Set current interlocutor rule.")
                .Perform("list topics", "ACTOR");

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
                                    if (System.Object.ReferenceEquals(thing, actor.GetPropertyOrDefault<MudObject>("interlocutor"))) return MatchPreference.VeryLikely;
                                    if (thing.GetPropertyOrDefault<bool>("actor?")) return MatchPreference.Likely;
                                    return MatchPreference.VeryUnlikely;
                                }),
                                OptionalKeyWord("ABOUT"),
                                FirstOf(
                                    Object("TOPIC", new TopicSource("NEW-LOCUTOR")),
                                    Rest("STRING-TOPIC"))),
                            Rest("STRING-TOPIC"))))
                .ID("Conversation:DiscussTopic")
                .Manual("Discusses the topic with whomever you are talking too.")
                .BeforeActing()
                .ProceduralRule((match, actor) =>
                {
                    if (match.ContainsKey("NEW-LOCUTOR"))
                    {
                        var newLocutor = match["NEW-LOCUTOR"] as MudObject;
                        if (Core.GlobalRules.ConsiderCheckRule("can converse?", actor, newLocutor) == CheckResult.Disallow) return PerformResult.Stop;
                        if (!System.Object.ReferenceEquals(newLocutor, actor.GetPropertyOrDefault<MudObject>("interlocutor")))
                        {
                            Core.GlobalRules.ConsiderPerformRule("greet", actor, newLocutor);
                            actor.SetProperty("interlocutor", newLocutor);
                        }
                    }
                    match.Upsert("LOCUTOR", actor.GetProperty<MudObject>("interlocutor"));
                    return PerformResult.Continue;
                }, "Implicitly greet new locutors rule.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.GetPropertyOrDefault<MudObject>("interlocutor") == null)
                    {
                        MudObject.SendMessage(actor, "@convo nobody");
                        return PerformResult.Stop;
                    }
                    return PerformResult.Continue;
                }, "Must be talking to someone rule.")
                .Check("can converse?", "ACTOR", "LOCUTOR")
                .Perform("discuss topic", "ACTOR", "LOCUTOR", "TOPIC")
                .AfterActing()
                .Perform("list topics", "ACTOR");

            Parser.AddCommand(
                KeyWord("TOPICS"))
                .ID("Conversation:Topic")
                .Manual("Lists topics currently available.")
                .Perform("list topics", "ACTOR");
        }
    }
}
