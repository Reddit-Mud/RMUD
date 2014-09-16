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
			var location = Actor.Location as Room;
			if (location == null) throw new InvalidOperationException("Error: Actor not in room.");

			if (Actor.ConnectedClient != null)
			{
				var builder = new StringBuilder();

				builder.Append(location.Short);
				builder.Append("\r\n");
				builder.Append(location.Long);
				builder.Append("\r\n");

				//Display objects in room
				if (location.Contents.Count > 0)
					builder.Append("Also here: " + String.Join(",", location.Contents.Select(t => t.Short)));
				else
					builder.Append("There is nothing here.");
				builder.Append("\r\n");

				//Display exits from room
				if (location.Links.Count > 0)
				{
					builder.Append("Obvious exits: " + String.Join(",", location.Links.Select(l => l.Direction.ToString())));
					builder.AppendLine("\r\n");
				}

				MudCore.SendEventMessage(Actor, EventMessageScope.Private, builder.ToString());
			}
		}
	}
}
