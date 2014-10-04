using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class OpenClose : CommandFactory
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
                                if (openable is OpenableRules && (openable as OpenableRules).Open)
                                    return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            }),
                        "I don't see that here.\r\n")),
				new CloseProcessor(),
				"Close someMudObject",
                "SUBJECT-SCORE");
		}
	}
	
	internal class CloseProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as OpenableRules;
			var MudObject = target as MudObject;
			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "I don't think the concept of 'open' and 'closed' applies to that.\r\n");
			}
			else
			{
                if (!Mud.IsVisibleTo(Actor, target as MudObject))
                {
                    if (Actor.ConnectedClient != null)
                        Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                    return;
                }

                var checkRule = target.CheckClose(Actor);
				if (checkRule.Allowed)
				{
                    if (target.HandleClose(Actor) == RuleHandlerFollowUp.Continue)
                    {
                        target.Open = false;

                        if (MudObject != null)
                        {
                            Mud.SendMessage(Actor, "You close " + MudObject.Definite + ".\r\n");
                            Mud.SendExternalMessage(Actor, Actor.Short + " closes " + MudObject.Definite + ".\r\n");
                        }
                    }

                    Mud.MarkLocaleForUpdate(target as MudObject);
				}
				else
				{
					Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
				}
			}
		}
	}

}
