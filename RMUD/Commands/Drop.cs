using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Drop : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("DROP", false),
					new ObjectMatcher("TARGET", new InScopeObjectSource())),
				new DropProcessor(),
				"Drop something");
		}
	}

	internal class DropProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as Thing;
			if (target == null)
			{
				if (Actor.ConnectedClient != null) Actor.ConnectedClient.Send("Drop what again?\r\n");
			}
			else
			{
				if (!Object.ReferenceEquals(target.Location, Actor))
				{
					Actor.ConnectedClient.Send("You aren't holding that.\r\n");
					return;
				}

				var dropRules = target as IDropRules;
				if (dropRules != null && !dropRules.CanDrop(Actor))
				{
					Actor.ConnectedClient.Send("You can't drop that.\r\n");
					return;
				}

				Mud.SendEventMessage(Actor, EventMessageScope.Single, "You drop " + target.Short + "\r\n");
				Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " drops " + target.Short + "\r\n");
				Thing.Move(target, Actor.Location);
			}
		}
	}
}
