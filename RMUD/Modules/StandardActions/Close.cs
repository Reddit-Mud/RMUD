using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
	internal class OpenClose : CommandFactory, DeclaresRules
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("CLOSE"),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, thing) =>
                                {
                                    if (GlobalRules.ConsiderCheckRuleSilently("can close?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                    return MatchPreference.Unlikely;
                                })))))
                .Manual("Closes a thing.")
                .Check("can close?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("closed", "ACTOR", "SUBJECT")
                .AfterActing();
		}

        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("closed", "[Actor, Item] : Handle the item being closed.");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .When((actor, item) => !GlobalRules.ConsiderValueRule<bool>("openable?", item))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .Do((actor, item) => CheckResult.Allow)
                .Name("Default close things rule.");

            GlobalRules.Perform<MudObject, MudObject>("closed").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "You close <the0>.", target);
                MudObject.SendExternalMessage(actor, "^<a0> closes <a1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
