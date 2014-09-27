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
					new KeyWord("OPEN", false),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                             (actor, openable) => {
                               if (openable is IOpenableRules && !(openable as IOpenableRules).Open)
                                     return 1;
                                return -1;
                            }, "SUBJECTSCORE"),
                        "I don't see that here.\r\n")),
				new OpenProcessor(),
				"Open something",
                "SUBJECTSCORE");

			Parser.AddCommand(
				new Sequence(
					new KeyWord("CLOSE", false),
                    new FailIfNoMatches(
		    			new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                            (actor, openable) =>
                            {
                                if (openable is IOpenableRules && (openable as IOpenableRules).Open)
                                    return 1;
                                return -1;
                            }, "SUBJECTSCORE"),
                        "I don't see that here.\r\n")),
				new CloseProcessor(),
				"Close something",
                "SUBJECTSCORE");
		}
	}
	
	internal class OpenProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as IOpenableRules;
			var thing = target as Thing;
			if (target == null)
			{
				if (Actor.ConnectedClient != null) 
					Mud.SendMessage(Actor, "I don't think the concept of 'open' and 'closed' applies to that.\r\n");
			}
			else
			{
                var checkRule = target.CanOpen(Actor);
				if (checkRule.Allowed)
				{
					if (thing != null)
					{
						Mud.SendMessage(Actor, MessageScope.Single, "You open " + thing.Definite + "\r\n");
						Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " opens " + thing.Definite + "\r\n");
						target.HandleOpen(Actor);
					}
				}
				else
				{
					Mud.SendMessage(Actor, MessageScope.Single, checkRule.ReasonDisallowed + "\r\n");
				}
			}
		}
	}

	internal class CloseProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as IOpenableRules;
			var thing = target as Thing;
			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "I don't think the concept of 'open' and 'closed' applies to that.\r\n");
			}
			else
			{
                var checkRule = target.CanClose(Actor);
				if (checkRule.Allowed)
				{
					if (thing != null)
					{
						Mud.SendMessage(Actor, MessageScope.Single, "You close " + thing.Definite + "\r\n");
						Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " closes " + thing.Definite + "\r\n");
						target.HandleClose(Actor);
					}
				}
				else
				{
					Mud.SendMessage(Actor, MessageScope.Single, checkRule.ReasonDisallowed + "\r\n");
				}
			}
		}
	}

}
