using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Go : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("GO", true),
					new Cardinal("DIRECTION")),
				new GoProcessor(),
				"Move between rooms.");
		}
	}

	internal class GoProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var direction = Match.Arguments["DIRECTION"] as Direction?;
			var location = Actor.Location as Room;
			var link = location.Links.FirstOrDefault(l => l.Direction == direction.Value);

			if (link == null)
				Mud.SendMessage(Actor, MessageScope.Single, "You can't go that way.\r\n");
			else
			{
				if (link.Door != null)
				{
					var openable = link.Door as IOpenableRules;
					if (openable != null && !openable.Open)
					{
						Mud.SendMessage(Actor, MessageScope.Single, "The door is closed.\r\n");
						return;
					}
				}

				Mud.SendMessage(Actor, MessageScope.Single, "You went " + direction.Value.ToString().ToLower() + ".\r\n");
				Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " went " + direction.Value.ToString().ToLower() + ".\r\n");
				var destination = Mud.GetObject(link.Destination, s =>
				{
					if (Actor.ConnectedClient != null)
						Mud.SendMessage(Actor, s + "\r\n");
				}) as Room;
				if (destination == null) throw new InvalidOperationException("[ERROR] Link does not lead to room.\r\n");
				Thing.Move(Actor, destination);
				Mud.EnqueuClientCommand(Actor.ConnectedClient, "look");

                var arriveMessage = Link.FromMessage(Link.Opposite(direction.Value));

				Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " arrives " + arriveMessage + ".\r\n");

                Mud.MarkLocaleForUpdate(location);
                Mud.MarkLocaleForUpdate(destination);
			}
		}
	}
}
