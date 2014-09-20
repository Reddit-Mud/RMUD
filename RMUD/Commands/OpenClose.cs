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
					new ObjectMatcher("TARGET", new InScopeObjectSource())),
				new OpenProcessor(),
				"Open something");

			Parser.AddCommand(
				new Sequence(
					new KeyWord("CLOSE", false),
					new ObjectMatcher("TARGET", new InScopeObjectSource())),
				new CloseProcessor(),
				"Close something");
		}
	}
	
	internal class OpenProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as IOpenableRules;
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
			var target = Match.Arguments["TARGET"] as IOpenableRules;
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
