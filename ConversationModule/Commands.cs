using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

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
                    MustMatch("Whom do you want to greet?",
                        Object("LOCUTOR", InScope, (actor, thing) =>
                        {
                            if (thing is NPC) return MatchPreference.VeryLikely;
                            else return MatchPreference.VeryUnlikely;
                        }))))
                .Manual("Initiates a conversation with the npc.")
                .Check("can converse?", "ACTOR", "LOCUTOR")
                .BeforeActing()
                .Perform("greet", "ACTOR", "LOCUTOR")
                .AfterActing()
                .ProceduralRule((match, actor) =>
                {
                    if (actor is Player)
                        actor.SetProperty("interlocutor", match["LOCUTOR"] as NPC);
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
                                    if (actor is Player && System.Object.ReferenceEquals(thing, actor.GetProperty<NPC>("interlocutor"))) return MatchPreference.VeryLikely;
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
                        if (Core.GlobalRules.ConsiderCheckRule("can converse?", actor, newLocutor) == CheckResult.Disallow) return PerformResult.Stop;
                        if (!System.Object.ReferenceEquals(newLocutor, actor.GetProperty<NPC>("interlocutor")))
                        {
                            Core.GlobalRules.ConsiderPerformRule("greet", actor, newLocutor);
                            actor.SetProperty("interlocutor", newLocutor as NPC);
                        }
                    }
                    match.Upsert("LOCUTOR", actor.GetProperty<NPC>("interlocutor"));
                    return PerformResult.Continue;
                }, "Implicitly greet new locutors rule.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.GetProperty<NPC>("interlocutor") == null)
                    {
                        MudObject.SendMessage(actor, "You aren't talking to anyone.");
                        return PerformResult.Stop;
                    }
                    return PerformResult.Continue;
                }, "Must be talking to someone rule.")
                .Check("can converse?", "ACTOR", "LOCUTOR")
                .Perform("discuss topic", "ACTOR", "LOCUTOR", "TOPIC")
                .Perform("list topics", "ACTOR");

            Parser.AddCommand(
                KeyWord("TOPICS"))
                .Manual("Lists topics currently available.")
                .Perform("list topics", "ACTOR");
        }
    }
}
