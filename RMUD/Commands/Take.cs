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
                    new FirstOf(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), 
                            (actor, thing) => {
                                if (actor.Contains(thing)) return -2;
                                if (thing is ITakeRules && !(thing as ITakeRules).CanTake(actor))
                                    return -1;
                                return 0;
                            }, "SUBJECTSCORE"),
                        new Rest("GARBAGE"))),
                new TakeProcessor(),
				"Take something",
                "SUBJECTSCORE");
		}
	}

	internal class TakeProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments.ValueOrDefault("SUBJECT") as Thing;
            if (target == null)
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "I don't see that here.\r\n");
            }
            else
            {
                if (Actor.Contains(target))
                {
                    Mud.SendMessage(Actor, "You are already holding that.\r\n");
                    return;
                }

                var takeRules = target as ITakeRules;
                if (takeRules != null && !takeRules.CanTake(Actor))
                {
                    Mud.SendMessage(Actor, "You can't take that.\r\n");
                    return;
                }

                var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
                if (takeRules != null) handleRuleFollowUp = takeRules.HandleTake(Actor);

                if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
                {
                    Mud.SendMessage(Actor, MessageScope.Single, "You take " + target.Indefinite + "\r\n");
                    Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " takes " + target.Indefinite + "\r\n");
                    Thing.Move(target, Actor);
                }
            }
		}
	}
}
