using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace IntroductionModule
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
                .Perform("introduce self", "ACTOR")
                .AfterActing();


            Parser.AddCommand(
                Sequence(
                    KeyWord("INTRODUCE"),
                    MustMatch("Introduce whom?",
                        Object("OBJECT", InScope, (actor, item) =>
                        {
                            if (item.GetPropertyOrDefault<bool>("actor?")) return MatchPreference.Likely;
                            return MatchPreference.Unlikely;
                        }))))
                .Manual("Introduces someone you know to everyone present. Now they will know them, too.")
                .Check("can introduce?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("introduce", "ACTOR", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("introduction memory", typeof(Dictionary<String, bool>), null);

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can introduce?", "[Actor A, Actor B] : Can A introduce B?", "actor", "itroductee");

            GlobalRules.Check<MudObject, MudObject>("can introduce?")
                .When((a, b) => !b.GetPropertyOrDefault<bool>("actor?"))
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
                .When((a, b) => !GlobalRules.ConsiderValueRule<bool>("actor knows actor?", a, b))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "How can you introduce <the0> when you don't know them yourself?", b);
                    return CheckResult.Disallow;
                })
                .Name("Can't introduce who you don't know rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .First
                .When((viewer, actor) => GlobalRules.ConsiderValueRule<bool>("actor knows actor?", viewer, actor))
                .Do((viewer, actor) =>
                {
                    MudObject.SendMessage(viewer, "^<the0>, a " + (actor.GetPropertyOrDefault<Gender>("gender") == Gender.Male ? "man." : "woman."), actor);
                    return PerformResult.Continue;
                })
                .Name("Report gender of known actors rule.");

            #region Perform and report rules

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("introduce", "[Actor A, Actor B] : Handle A introducing B.", "actor", "introductee");

            GlobalRules.Perform<MudObject, MudObject>("introduce")
                .Do((actor, introductee) =>
                {
                    var locale = MudObject.FindLocale(introductee);
                    if (locale != null)
                        foreach (var player in MudObject.EnumerateObjectTree(locale).Where(o => o.GetPropertyOrDefault<bool>("actor?")))
                            GlobalRules.ConsiderPerformRule("introduce to", introductee, player);

                    MudObject.SendExternalMessage(actor, "^<the0> introduces <the1>.", actor, introductee);
                    MudObject.SendMessage(actor, "You introduce <the0>.", introductee);
                    return PerformResult.Continue;
                })
                .Name("Report introduction rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject>("introduce self", "[Introductee] : Introduce the introductee");

            GlobalRules.Perform<MudObject>("introduce self")
                .Do((introductee) =>
                {
                    var locale = MudObject.FindLocale(introductee);
                    if (locale != null)
                        foreach (var player in MudObject.EnumerateObjectTree(locale).Where(o => o.GetPropertyOrDefault<bool>("actor?")))
                            GlobalRules.ConsiderPerformRule("introduce to", introductee, player);

                    MudObject.SendExternalMessage(introductee, "^<the0> introduces themselves.", introductee);
                    MudObject.SendMessage(introductee, "You introduce yourself.");

                    return PerformResult.Continue;
                })
                .Name("Introduce an actor to everyone present rule.");

            #endregion

            #region Printed name rules

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing.GetPropertyOrDefault<bool>("actor?"))
                .When((viewer, thing, article) => GlobalRules.ConsiderValueRule<bool>("actor knows actor?", viewer, thing))
                .Do((viewer, actor, article) => actor.GetProperty<String>("short"))
                .Name("Name of introduced actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing.GetPropertyOrDefault<bool>("actor?"))
                .When((viewer, thing, article) => thing.GetPropertyOrDefault<Gender>("gender") == Gender.Male)
                .Do((viewer, actor, article) => article + " man")
                .Name("Default name for unintroduced male actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing.GetPropertyOrDefault<bool>("actor?"))
                .When((viewer, thing, article) => thing.GetPropertyOrDefault<Gender>("gender") == Gender.Female)
                .Do((viewer, actor, article) => article + " woman")
                .Name("Default name for unintroduced female actor.");

            #endregion

            #region Knowledge management rules

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("actor knows actor?", "[Player, Whom] : Does the player know the actor?");

            GlobalRules.Value<MudObject, MudObject, bool>("actor knows actor?")
                .Do((player, whom) => RecallActor(player, whom))
                .Name("Use player memory to recall actors rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("introduce to", "[Introductee, ToWhom] : Introduce the introductee to someone");

            GlobalRules.Perform<MudObject, MudObject>("introduce to")
                .Do((introductee, player) =>
                {
                    RememberActor(player, introductee);
                    return PerformResult.Continue;
                })
                .Name("Players remember actors rule.");
                        
            #endregion
        }

        private static void RememberActor(MudObject Player, MudObject Actor)
        {
            if (!Player.HasProperty("introduction memory"))
                Player.SetProperty("introduction memory", new Dictionary<string, bool>());
            Player.GetProperty<Dictionary<String, bool>>("introduction memory").Upsert(Actor.Path, true);
        }

        private static bool RecallActor(MudObject Player, MudObject Actor)
        {
            if (!Player.HasProperty("introduction memory")) return false;
            var memory = Player.GetProperty<Dictionary<String, bool>>("introduction memory");
            if (!memory.ContainsKey(Actor.Path)) return false;
            return memory[Actor.Path];
        }
    }
}
