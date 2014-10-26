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
                        "I don't know what object you're talking about.")),
				new DropProcessor(),
				"Drop something",
                "SUBJECT-SCORE");
		}
	}

	internal class DropProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;

            if (!Mud.ObjectContainsObject(Actor, target))
            {
                Mud.SendMessage(Actor, "You aren't holding that.");
                return;
            }

            var dropRules = target as DropRules;
            if (dropRules != null)
            {
                var checkRule = dropRules.Check(Actor);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed);
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (dropRules != null) handleRuleFollowUp = dropRules.Handle(Actor);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, "You drop " + target.Indefinite + ".");
                Mud.SendExternalMessage(Actor, Actor.Short + " drops " + target.Indefinite + ".");
                MudObject.Move(target, Actor.Location);
            }

            if (Actor.Location != null)
            {
                foreach (var witness in Mud.GatherObjects<WitnessDropRules>(Actor.Location, obj =>
                {
                    if (Object.ReferenceEquals(obj, target)) return null;
                    return obj as WitnessDropRules;
                })) witness.Handle(Actor, target);
            }

            Mud.MarkLocaleForUpdate(target);
        }
	}
}
