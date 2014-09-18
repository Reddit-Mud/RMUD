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
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "You can't go that way.\r\n");
			else
			{
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "You went " + direction.Value.ToString().ToLower() + ".\r\n");
				Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " went " + direction.Value.ToString().ToLower() + "\r\n");
				var destination = Mud.GetObject(link.Destination) as Room;
				if (destination == null) throw new InvalidOperationException("Link does not lead to room.");
				MudObject.Move(Actor, destination);
				Mud.EnqueuClientCommand(Actor.ConnectedClient, "look");
			}
		}
	}
}
