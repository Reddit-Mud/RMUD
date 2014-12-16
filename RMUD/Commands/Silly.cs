using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{

    internal class Silly : CommandFactory, DeclaresRules
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                 new Sequence(
                    new KeyWord("SILLY"),
                     new FailIfNoMatches(
                         new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                             {
                                 if (Object is RMUD.Actor) return MatchPreference.Likely;
                                 else return MatchPreference.Unlikely;
                             }),
                         "Silly whom?")),
                 new SillyProcessor(),
                 "SILLY SILLY SILLY");

            Parser.AddCommand(
                new KeyWord("DANCE"),
                new DanceProcessor(),
                "Do a silly dance.");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("is-silly", "Determine if an object is silly.");
            GlobalRules.Value<MudObject, bool>("is-silly").Last.Do((thing) => false).Name("Things are serious by default rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-silly", "[actor, target] : Can the actor make the target silly?");

            GlobalRules.Check<MudObject, MudObject>("can-silly").First
                .When((actor, target) => !(target is Actor))
                .Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "That just sounds silly.");
                    return CheckResult.Disallow;
                })
                .Name("Can only silly actors rule.");

            GlobalRules.Check<MudObject, MudObject>("can-silly")
                .Do((actor, target) => GlobalRules.IsVisibleTo(actor, target))
                .Name("Silly target must be visible.");

            GlobalRules.Check<MudObject, MudObject>("can-silly")
                .When((actor, target) => GlobalRules.ConsiderValueRule<bool>("is-silly", target, target))
                .Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "^<the0> is already silly.", target);
                    return CheckResult.Disallow;
                })
                .Name("Can't silly if already silly rule.");

            GlobalRules.Check<MudObject, MudObject>("can-silly")
                .Last
                .Do((actor, target) => CheckResult.Allow)
                .Name("Go ahead and apply silly then rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("apply-silly", "[actor, target] : Apply silly status.");

            GlobalRules.Perform<MudObject, MudObject>("apply-silly")
                .Do((actor, target) =>
                {
                    Mud.SendExternalMessage(actor, "^<the0> applies extra silly to <the1>.", actor, target);
                    Mud.SendMessage(actor, "You apply extra silly to <the0>.", target);

                    var ruleID = Guid.NewGuid();
                    var counter = 100;

                    target.Nouns.Add("silly");

                    target.Value<MudObject, bool>("is-silly").Do((thing) => true).ID(ruleID.ToString());

                    target.Value<MudObject, MudObject, String, String>("printed-name")
                        .Do((viewer, thing, article) =>
                        {
                            return "silly " + thing.Short;
                        })
                        .Name("Silly name rule")
                        .ID(ruleID.ToString());

                    GlobalRules.Perform("heartbeat")
                        .Do(() =>
                        {
                            counter -= 1;
                            if (counter <= 0)
                            {
                                Mud.SendExternalMessage(target, "^<the0> is serious now.", target);
                                target.Nouns.Remove("silly");
                                target.Rules.DeleteAll(ruleID.ToString());
                                GlobalRules.DeleteRule("heartbeat", ruleID.ToString());
                            }
                            return PerformResult.Continue;
                        })
                        .ID(ruleID.ToString());

                    return PerformResult.Continue;
                })
                .Name("Apply sillyness rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject>("can-dance", "[actor] : Can the actor dance?");

            GlobalRules.Check<MudObject>("can-dance")
                .When(actor => !GlobalRules.ConsiderValueRule<bool>("is-silly", actor, actor))
                .Do(actor =>
                {
                    Mud.SendMessage(actor, "You don't feel silly enough for that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't dance when not silly rule.");

            GlobalRules.Check<MudObject>("can-dance")
                .Last
                .Do(actor => CheckResult.Allow)
                .Name("You can dance if you want to rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject>("perform-dance", "[actor] : Perform a silly dance.");

            GlobalRules.Perform<MudObject>("perform-dance")
                .Do(actor =>
                {
                    Mud.SendExternalMessage(actor, "^<the0> does a very silly dance.", actor);
                    Mud.SendMessage(actor, "You do a very silly dance.");
                    return PerformResult.Continue;
                })
                .Name("If your friends don't dance rule.");
        }
    }

    internal class SillyProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["OBJECT"] as MudObject;

            if (GlobalRules.ConsiderCheckRule("can-silly", target, Actor, target) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("apply-silly", target, Actor, target);
        }
    }

    internal class DanceProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (GlobalRules.ConsiderCheckRule("can-dance", Actor, Actor) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("perform-dance", Actor, Actor);
        }
    }
}
