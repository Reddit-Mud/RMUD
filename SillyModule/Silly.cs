using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace SillyModule
{

    internal class Silly : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("SILLY"),
                    MustMatch("Who is being too damn serious?",
                        Object("OBJECT", InScope, (actor, item) =>
                            {
                                if (item is RMUD.Actor) return MatchPreference.Likely;
                                else return MatchPreference.Unlikely;
                            }))))
                 .Manual("Applies the silly status effect to the target of your choice. Being silly will make it safe for your victim to dance. Sillification is meant as a demonstration of the concepts involved with rule books and status effects, and not as an actual component of the game world.")
                 .Check("can silly?", "ACTOR", "OBJECT")
                 .Perform("silly", "ACTOR", "OBJECT");

            Parser.AddCommand(
                KeyWord("DANCE"))
                .Manual(
                @"We can dance if we want to
We can leave your friends behind
'Cause your friends don't dance and if they don't dance
Well they're no friends of mine
I say, we can go where we want to
A place where they will never find
And we can act like we come from out of this world
Leave the real one far behind
And we can dance")
                .Check("can dance?", "ACTOR")
                .Perform("dance", "ACTOR");
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("silly?", "[Thing -> bool] : Determine if an object is silly.", "item");
            GlobalRules.Value<MudObject, bool>("silly?").Last.Do((thing) => false).Name("Things are serious by default rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can silly?", "[Actor, Target] : Can the actor make the target silly?", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can silly?").First
                .When((actor, target) => !(target is Actor))
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "That just sounds silly.");
                    return CheckResult.Disallow;
                })
                .Name("Can only silly actors rule.");

            GlobalRules.Check<MudObject, MudObject>("can silly?")
                .Do((actor, target) => MudObject.CheckIsVisibleTo(actor, target))
                .Name("Silly target must be visible.");

            GlobalRules.Check<MudObject, MudObject>("can silly?")
                .When((actor, target) => GlobalRules.ConsiderValueRule<bool>("silly?", target))
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "^<the0> is already silly.", target);
                    return CheckResult.Disallow;
                })
                .Name("Can't silly if already silly rule.");

            GlobalRules.Check<MudObject, MudObject>("can silly?")
                .Last
                .Do((actor, target) => CheckResult.Allow)
                .Name("Let the silliness ensue rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("silly", "[Actor, Target] : Apply silly status to the target.", "actor", "item");

            GlobalRules.Perform<MudObject, MudObject>("silly")
                .Do((actor, target) =>
                {
                    MudObject.SendExternalMessage(actor, "^<the0> applies extra silly to <the1>.", actor, target);
                    MudObject.SendMessage(actor, "You apply extra silly to <the0>.", target);

                    var ruleID = Guid.NewGuid();
                    var counter = 100;

                    target.Nouns.Add("silly");

                    target.Value<MudObject, bool>("silly?").Do((thing) => true).ID(ruleID.ToString())
                        .Name("Silly things are silly rule.");

                    target.Value<MudObject, MudObject, String, String>("printed name")
                        .Do((viewer, thing, article) =>
                        {
                            return "silly " + thing.Short;
                        })
                        .Name("Silly things have silly names rule.")
                        .ID(ruleID.ToString());

                    GlobalRules.Perform("heartbeat")
                        .Do(() =>
                        {
                            counter -= 1;
                            if (counter <= 0)
                            {
                                MudObject.SendExternalMessage(target, "^<the0> is serious now.", target);
                                target.Nouns.Remove("silly");
                                target.Rules.DeleteAll(ruleID.ToString());
                                GlobalRules.DeleteRule("heartbeat", ruleID.ToString());
                            }
                            return PerformResult.Continue;
                        })
                        .ID(ruleID.ToString())
                        .Name("Countdown to seriousness rule.");

                    return PerformResult.Continue;
                })
                .Name("Apply sillyness rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject>("can dance?", "[Actor] : Can the actor dance?", "actor");

            GlobalRules.Check<MudObject>("can dance?")
                .When(actor => !GlobalRules.ConsiderValueRule<bool>("silly?", actor))
                .Do(actor =>
                {
                    MudObject.SendMessage(actor, "You don't feel silly enough for that.");
                    return CheckResult.Disallow;
                })
                .Name("Your friends don't dance rule.");

            GlobalRules.Check<MudObject>("can dance?")
                .Last
                .Do(actor => CheckResult.Allow)
                .Name("You can dance if you want to rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject>("dance", "[Actor] : Perform a silly dance.", "actor");

            GlobalRules.Perform<MudObject>("dance")
                .Do(actor =>
                {
                    MudObject.SendExternalMessage(actor, "^<the0> does a very silly dance.", actor);
                    MudObject.SendMessage(actor, "You do a very silly dance.");
                    return PerformResult.Continue;
                })
                .Name("They aren't no friends of mine rule.");
        }
    }
}
