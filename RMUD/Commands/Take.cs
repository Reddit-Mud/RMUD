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
					new ObjectMatcher("SUBJECT", new InScopeObjectSource(), 
                        (actor, thing) => {
                            if (actor.Contains(thing)) return -2;
                            if (thing is ITakeRules && !(thing as ITakeRules).CanTake(actor))
                                return -1;
                            return 0;
                        }, "SUBJECTSCORE")),
                new TakeProcessor(),
				"Take something",
                "SUBJECTSCORE");
		}
	}

	internal class TakeProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as Thing;
            if (target == null)
            {
                if (Actor.ConnectedClient != null) Actor.ConnectedClient.Send("Take what again?\r\n");
            }
            else
            {
                if (Actor.Contains(target))
                {
                    Actor.ConnectedClient.Send("You are already holding that.\r\n");
                    return;
                }

                var takeRules = target as ITakeRules;
                if (takeRules != null && !takeRules.CanTake(Actor))
                {
                    Actor.ConnectedClient.Send("You can't take that.\r\n");
                    return;
                }

                var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
                if (takeRules != null) handleRuleFollowUp = takeRules.HandleTake(Actor);

                if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
                {
                    Mud.SendEventMessage(Actor, EventMessageScope.Single, "You take " + target.Indefinite + "\r\n");
                    Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " takes " + target.Indefinite + "\r\n");
                    Thing.Move(target, Actor);
                }
            }
		}
	}
}
