using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
	internal class OpenClose : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("CLOSE"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, thing) =>
                                {
                                    if (Core.GlobalRules.ConsiderCheckRuleSilently("can close?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                    return MatchPreference.Unlikely;
                                })))))
                .Manual("Closes a thing.")
                .Check("can close?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("closed", "ACTOR", "SUBJECT")
                .AfterActing();
		}

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("you close", "You close <the0>.");
            Core.StandardMessage("they close", "^<the0> closes <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("closed", "[Actor, Item] : Handle the item being closed.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .When((actor, item) => !item.GetBooleanProperty("openable?"))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "@not openable");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?")
                .Do((actor, item) => CheckResult.Allow)
                .Name("Default close things rule.");

            GlobalRules.Perform<MudObject, MudObject>("closed").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "@you close", target);
                MudObject.SendExternalMessage(actor, "@they close", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<MudObject, MudObject>("can close?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
