using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.ActorIntroduction
{
    internal class Introduce : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("INTRODUCE"),
                    Or(
                        KeyWord("MYSELF"),
                        KeyWord("ME"),
                        KeyWord("SELF"))))
                .Manual("This is a specialized version of the commad to handle 'introduce me'.")
                .BeforeActing()
                .ProceduralRule((match, actor) =>
                {
                    MudObject.Introduce(actor);
                    MudObject.SendExternalMessage(actor, "^<the0> introduces themselves.", actor);
                    MudObject.SendMessage(actor, "You introduce yourself.");
                    return PerformResult.Continue;
                }, "Introduce yourself rule.")
                .AfterActing();


            Parser.AddCommand(
                Sequence(
                    KeyWord("INTRODUCE"),
                    MustMatch("Introduce whom?",
                        Object("OBJECT", InScope, (actor, item) =>
                        {
                            if (item is Actor) return MatchPreference.Likely;
                            return MatchPreference.Unlikely;
                        }))))
                .Manual("Introduces someone you know to everyone present. Now they will know them, too.")
                .Check("can introduce?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("introduce", "ACTOR", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can introduce?", "[Actor A, Actor B] : Can A introduce B?");

            GlobalRules.Check<MudObject, MudObject>("can introduce?")
                .When((a, b) => !(b is Actor))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "That just sounds silly.");
                    return CheckResult.Disallow;
                })
                .Name("Can only introduce actors rule.");

            GlobalRules.Check<MudObject, MudObject>("can introduce?")
                .Do((a, b) => MudObject.CheckIsVisibleTo(a, b))
                .Name("Introducee must be visible rule.");

            GlobalRules.Check<MudObject, MudObject>("can introduce?")
                .When((a, b) => !MudObject.ActorKnowsActor(a as Actor, b as Actor))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "How can you introduce <the0> when you don't know them yourself?", b);
                    return CheckResult.Disallow;
                })
                .Name("Can't introduce who you don't know rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("introduce", "[Actor A, Actor B] : Handle A introducing B.");

            GlobalRules.Perform<MudObject, MudObject>("introduce")
                .Do((a, b) =>
                {
                    MudObject.Introduce(b as Actor);
                    MudObject.SendExternalMessage(a, "^<the0> introduces <the1>.", a, b);
                    MudObject.SendMessage(a, "You introduce <the0>.", b);
                    return PerformResult.Continue;
                })
                .Name("Report introduction rule.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => viewer is Actor && thing is Actor && MudObject.ActorKnowsActor(viewer as Actor, thing as Actor))
                .Do((viewer, actor, article) => actor.Short)
                .Name("Name of introduced actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing is Actor && (thing as Actor).Gender == Gender.Male)
                .Do((viewer, actor, article) => article + " man")
                .Name("Default name for unintroduced male actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing is Actor && (thing as Actor).Gender == Gender.Female)
                .Do((viewer, actor, article) => article + " woman")
                .Name("Default name for unintroduced female actor.");
        }
    }
}
