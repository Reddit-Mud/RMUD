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
                    new FailIfNoMatches(
		    			new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                            (actor, openable) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can-close", openable, actor, openable) == CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            }),
                        "I don't see that here.")),
				new CloseProcessor(),
				"Close something.",
                "SUBJECT-SCORE");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-close", "[Actor, Item] - determine if the item can be closed.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("on-closed", "Item based rulebook to handle the item being closed.");

            GlobalRules.Check<MudObject, MudObject>("can-close").Do((a, b) =>
            {
                Mud.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                return CheckResult.Disallow;
            }).Name("Default can't close unopenable things rule.");

            GlobalRules.Perform<MudObject, MudObject>("on-closed").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You close <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> closes <a1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");
        }
    }
	
	internal class CloseProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            if (GlobalRules.ConsiderCheckRule("can-close", target, Actor, target) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("on-closed", target, Actor, target);
        }
	}

}
