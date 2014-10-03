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
	
	internal class CloseProcessor : ICommandProcessor
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
                            Mud.SendMessage(Actor, MessageScope.Single, "You close " + MudObject.Definite + ".\r\n");
                            Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " closes " + MudObject.Definite + ".\r\n");

                            var source = Match.Arguments["SUBJECT-SOURCE"] as String;
                            if (source == "LINK")
                            {
                                var location = Actor.Location as Room;
                                var link = location.Links.FirstOrDefault(l => Object.ReferenceEquals(target, l.Portal));
                                if (link != null)
                                {
                                    var otherRoom = Mud.GetObject(link.Destination);
                                    if (otherRoom != null)
                                    {
                                        Mud.SendMessage(otherRoom as Room, String.Format("{0} closes {1}.\r\n", Actor.Short, MudObject.Definite));
                                        Mud.MarkLocaleForUpdate(otherRoom);
                                    }
                                }
                            }
                        }
                    }

                    Mud.MarkLocaleForUpdate(target as MudObject);
				}
				else
				{
					Mud.SendMessage(Actor, MessageScope.Single, checkRule.ReasonDisallowed + "\r\n");
				}
			}
		}
	}

}
