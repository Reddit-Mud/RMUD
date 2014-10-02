using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Take : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("GET", false),
						new KeyWord("TAKE", false)),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), 
                            (actor, thing) => {
                                if (actor.Contains(thing, RelativeLocations.Held)) return -2;
                                //Prefer things that can actually be taken
                                if (thing is TakeRules && !(thing as TakeRules).Check(actor).Allowed)
                                    return -1;
                                return 0;
                            }),
                        "I don't see that here.\r\n")),
                new TakeProcessor(),
				"Take something",
                "SUBJECT-SCORE");
		}
	}

	internal class TakeProcessor : ICommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments.ValueOrDefault("SUBJECT") as Thing;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                return;
            }

            if (Actor.Contains(target, RelativeLocations.Held))
            {
                Mud.SendMessage(Actor, "You are already holding that.\r\n");
                return;
            }

            var takeRules = target as TakeRules;
            if (takeRules != null)
            {
                var checkRule = takeRules.Check(Actor);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (takeRules != null) handleRuleFollowUp = takeRules.Handle(Actor);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, MessageScope.Single, "You take " + target.Indefinite + "\r\n");
                Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " takes " + target.Indefinite + "\r\n");
                Thing.Move(target, Actor);
            }

            Mud.MarkLocaleForUpdate(target);
        }
	}
}
