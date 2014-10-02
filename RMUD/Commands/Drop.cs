using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Drop : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("DROP", false),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I don't know what object you're talking about.\r\n")),
				new DropProcessor(),
				"Drop something",
                "SUBJECT-SCORE");
		}
	}

	internal class DropProcessor : ICommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as Thing;

            if (!Mud.ObjectContainsObject(Actor, target))
            {
                Mud.SendMessage(Actor, "You aren't holding that.\r\n");
                return;
            }

            var dropRules = target as DropRules;
            if (dropRules != null)
            {
                var checkRule = dropRules.Check(Actor);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (dropRules != null) handleRuleFollowUp = dropRules.Handle(Actor);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, MessageScope.Single, "You drop " + target.Indefinite + "\r\n");
                Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " drops " + target.Indefinite + "\r\n");
                Thing.Move(target, Actor.Location);
            }

            Mud.MarkLocaleForUpdate(target);
        }
	}
}
