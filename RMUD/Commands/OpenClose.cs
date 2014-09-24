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
					new ObjectMatcher("OBJECT", new InScopeObjectSource(),
                         (actor, openable) => {
                             if (openable is IOpenableRules && !(openable as IOpenableRules).Open)
                                 return 1;
                            return -1;
                        }, "SUBJECTSCORE")),
				new OpenProcessor(),
				"Open something",
                "SUBJECTSCORE");

			Parser.AddCommand(
				new Sequence(
					new KeyWord("CLOSE", false),
					new ObjectMatcher("OBJECT", new InScopeObjectSource(),
                        (actor, openable) =>
                        {
                            if (openable is IOpenableRules && (openable as IOpenableRules).Open)
                                return 1;
                            return -1;
                        }, "SUBJECTSCORE")),
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
					Actor.ConnectedClient.Send("I don't think the concept of 'open' and 'closed' applies to that.\r\n");
			}
			else
			{
				if (target.CanOpen(Actor))
				{
					if (thing != null)
					{
						Mud.SendEventMessage(Actor, EventMessageScope.Single, "You open " + thing.Definite + "\r\n");
						Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " opens " + thing.Definite + "\r\n");
						target.HandleOpen(Actor);
					}
				}
				else
				{
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You can't open that.\r\n");
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
					Actor.ConnectedClient.Send("I don't think the concept of 'open' and 'closed' applies to that.\r\n");
			}
			else
			{
				if (target.CanClose(Actor))
				{
					if (thing != null)
					{
						Mud.SendEventMessage(Actor, EventMessageScope.Single, "You close " + thing.Definite + "\r\n");
						Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " closes " + thing.Definite + "\r\n");
						target.HandleClose(Actor);
					}
				}
				else
				{
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You can't close that.\r\n");
				}
			}
		}
	}

}
