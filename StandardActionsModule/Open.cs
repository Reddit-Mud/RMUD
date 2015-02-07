using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
	internal class Open : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("OPEN"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, thing) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can open?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            })))))
                .ID("StandardActions:Open")
                .Manual("Opens an openable thing.")
                .Check("can open?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("opened", "ACTOR", "SUBJECT")
                .AfterActing();
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("not openable", "I don't think the concept of 'open' applies to that.");
            Core.StandardMessage("you open", "You open <the0>.");
            Core.StandardMessage("they open", "^<the0> opens <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can open?", "[Actor, Item] : Can the actor open the item?", "actor", "item");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("openable?", "[Item -> bool] : Is the item openable?", "item");
            
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("opened", "[Actor, Item] : Handle the actor opening the item.", "actor", "item");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("open?", "[Item -> bool] : Is the item open?");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .When((actor, item) => !GlobalRules.ConsiderValueRule<bool>("openable?", item))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "@not openable");
                    return CheckResult.Disallow;
                })
                .Name("Can't open the unopenable rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default go ahead and open it rule.");

            GlobalRules.Value<MudObject, bool>("open?").Do(a => true).Name("Things open by default rule.");

            GlobalRules.Value<MudObject, bool>("openable?").Do(a => false).Name("Things unopenable by default rule.");

            GlobalRules.Perform<MudObject, MudObject>("opened").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "@you open", target);
                MudObject.SendExternalMessage(actor, "@they open", actor, target);
                return PerformResult.Continue;
            }).Name("Default report opening rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
