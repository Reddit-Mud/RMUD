using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Look : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			var matcher = new KeyWord("LOOK", false);
			Parser.AddCommand(matcher, new LookProcessor());
		}
	}

	internal class LookProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var location = MudCore.Database.LoadObject(Actor.Location) as Room;

			if (Actor.ConnectedClient != null)
			{
				//Send main room description
				MudCore.SendMessage(Actor.ConnectedClient, location.Short + "\n" + location.Long + "\n\n", false);
			
				//Display objects in room
				if (location.Contents.Count > 0)
					MudCore.SendMessage(Actor.ConnectedClient, 
						"Also here: " + String.Join(",", location.Contents.Select(t => t.Short)) + "\n", false);
				else
					MudCore.SendMessage(Actor.ConnectedClient, "There is nothing here.\n");

				//Display exits from room
				if (location.Links.Count > 0)
				{
					MudCore.SendMessage(Actor.ConnectedClient,
						"Obvious exits: " + String.Join(",", location.Links.Select(l => l.Direction.ToString())) + "\n",
						false);
				}
			}
		}
	}
}
