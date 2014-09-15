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
				var builder = new StringBuilder();

				builder.Append(location.Short);
				builder.AppendLine();
				builder.Append(location.Long);
				builder.AppendLine();

				//Display objects in room
				if (location.Contents.Count > 0)
					builder.Append("Also here: " + String.Join(",", location.Contents.Select(t => t.Short)));
				else
					builder.Append("There is nothing here.");
				builder.AppendLine();

				//Display exits from room
				if (location.Links.Count > 0)
					builder.Append("Obvious exits: " + String.Join(",", location.Links.Select(l => l.Direction.ToString())));

				MudCore.SendEventMessage(Actor, EventMessageScope.Private, builder.ToString());
			}
		}
	}
}
