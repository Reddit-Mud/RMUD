using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class OpenClose : CommandFactory, DeclaresRules
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("CLOSE", false),
                    new ScoreGate(
                        new FailIfNoMatches(
                            new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                                (actor, openable) =>
                                {
                                    if (GlobalRules.ConsiderCheckRuleSilently("can-close", openable, actor, openable) == CheckResult.Allow) return MatchPreference.Likely;
                                    return MatchPreference.Unlikely;
                                }),
                            "I don't see that here."),
                        "SUBJECT")),
                "Close something.")
                .Check("can close?", "SUBJECT", "ACTOR", "SUBJECT")
                .Perform("closed", "SUBJECT", "ACTOR", "SUBJECT");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("closed", "[Actor, Item] : Handle the item being closed.");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .When((actor, item) => !GlobalRules.ConsiderValueRule<bool>("openable?", item, item))
                .Do((a, b) =>
                {
                    Mud.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .Do((actor, item) => CheckResult.Allow)
                .Name("Default close things rule.");

            GlobalRules.Perform<MudObject, MudObject>("closed").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You close <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> closes <a1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?").First.Do((actor, item) => GlobalRules.IsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
