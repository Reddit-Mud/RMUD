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
				new GoProcessor());
		}
	}

	internal class GoProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var direction = Match.Arguments["DIRECTION"] as Direction?;
			var location = MudCore.Database.LoadObject(Actor.Location) as Room;
			var link = location.Links.FirstOrDefault(l => l.Direction == direction.Value);

			if (link == null)
				MudCore.SendMessage(Actor.ConnectedClient, "You can't go that way.\n", false);
			else
			{
				Actor.Location = link.Destination;
				MudCore.EnqueuClientCommand(Actor.ConnectedClient, "look");
			}
		}
	}
}
