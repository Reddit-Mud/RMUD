using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Put : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("PUT", false),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "You don't seem to have that.\r\n"),
                    new RelativeLocationMatcher("RELLOC"),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                            {
                                //Prefer objects that are actually containers. No means curently to prefer
                                //objects that actually support the relloc we matched previously.
                                if (Object is IContainer) return 1;
                                return 0;
                            }),
                        "I can't see that here.")),
				new PutProcessor(),
				"Put something on, in, under or behind something",
                "SUBJECT-SCORE",
                "OBJECT-SCORE");
		}
	}

	internal class PutProcessor : ICommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as Thing;

            if (!Actor.Contains(target, RelativeLocations.Held))
            {
                Mud.SendMessage(Actor, "You aren't holding that.\r\n");
                return;
            }

            var relloc = Match.Arguments["RELLOC"] as RelativeLocations?;

            var container = Match.Arguments["OBJECT"] as IContainer;
            if (container == null || ((container.LocationsSupported & relloc.Value) != relloc.Value))
            {
                Mud.SendMessage(Actor, String.Format("You can't put things {0} that.\r\n", Mud.RelativeLocationName(relloc.Value)));
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

            var putRules = container as PutRules;
            if (putRules != null)
            {
                var checkRule = putRules.Check(Actor, target, relloc.Value);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (putRules != null) handleRuleFollowUp = putRules.Handle(Actor, target, relloc.Value);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, MessageScope.Single, String.Format("You put {0} {1} {2}.\r\n", target.Definite, Mud.RelativeLocationName(relloc.Value), (container as Thing).Definite));
                Mud.SendMessage(Actor, MessageScope.External, String.Format("{0} puts {1} {2} {3}.\r\n", Actor.Short, target.Indefinite, Mud.RelativeLocationName(relloc.Value), (container as Thing).Definite));
                Thing.Move(target, container as MudObject, relloc.Value);
            }

            Mud.MarkLocaleForUpdate(target);

        }
	}
}
