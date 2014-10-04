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
                    new Optional(
                        new RelativeLocationMatcher("RELLOC")),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                            {
                                //Prefer objects that are actually containers. No means curently to prefer
                                //objects that actually support the relloc we matched previously.
                                if (Object is Container) return MatchPreference.Likely;
                                return MatchPreference.Plausible;
                            }),
                        "I can't see that here.")),
				new PutProcessor(),
				"Put someMudObject on, in, under or behind someMudObject",
                "SUBJECT-SCORE",
                "OBJECT-SCORE");
		}
	}

	internal class PutProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;
            var container = Match.Arguments["OBJECT"] as Container;

            if (!Mud.IsVisibleTo(Actor, container as MudObject))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                return;
            }

            if (!Mud.ObjectContainsObject(Actor, target as MudObject))
            {
                Mud.SendMessage(Actor, "You aren't holding that.\r\n");
                return;
            }
            
            RelativeLocations relloc = RelativeLocations.In;
            if (Match.Arguments.ContainsKey("RELLOC"))
                relloc = (Match.Arguments["RELLOC"] as RelativeLocations?).Value;
            else
            {
                if (container != null) relloc = container.DefaultLocation;
            }

            if (container == null || ((container.LocationsSupported & relloc) != relloc))
            {
                Mud.SendMessage(Actor, String.Format("You can't put MudObjects {0} that.\r\n", Mud.RelativeLocationName(relloc)));
                return;
            }

            if (relloc == RelativeLocations.In)
            {
                var openable = target as OpenableRules;
                if (openable != null && !openable.Open)
                {
                    Mud.SendMessage(Actor, Mud.CapFirst(String.Format("{0} is closed.\r\n", target.Definite)));
                    return;
                }
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
                var checkRule = putRules.Check(Actor, target, relloc);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (putRules != null) handleRuleFollowUp = putRules.Handle(Actor, target, relloc);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, String.Format("You put {0} {1} {2}.\r\n", target.Definite, Mud.RelativeLocationName(relloc), (container as MudObject).Definite));
                Mud.SendExternalMessage(Actor, String.Format("{0} puts {1} {2} {3}.\r\n", Actor.Short, target.Indefinite, Mud.RelativeLocationName(relloc), (container as MudObject).Definite));
                MudObject.Move(target, container as MudObject, relloc);
            }

            Mud.MarkLocaleForUpdate(target);

        }
	}
}
