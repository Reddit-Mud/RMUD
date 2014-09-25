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
			Parser.AddCommand(
				new KeyWord("LOOK", false),
				new LookProcessor(),
				"Look around at your suroundings.");
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
				builder.Append(location.Long.Expand(Actor, location));
				builder.Append("\r\n");

                var visibleThings = new List<Thing>(location.Contents.Where(t => !Object.ReferenceEquals(t, Actor)));

				//Display objects in room
				if (visibleThings.Count > 0)
					builder.Append("Also here: " + String.Join(", ", visibleThings.Select(t => t.Indefinite)));
				else
					builder.Append("There is nothing here.");
				builder.Append("\r\n");

				//Display exits from room
				if (location.Links.Count > 0)
				{
					builder.Append("Obvious exits:\r\n");

                    foreach (var link in location.Links)
                    {
                        builder.Append("  ");
                        builder.Append(Mud.CapFirst(link.Direction.ToString()));

                        if (link.Door != null && link.Door is Thing)
                        {
                            builder.Append(", through ");
                            builder.Append((link.Door as Thing).Indefinite);
                        }

                        var destinationRoom = Mud.GetObject(link.Destination) as Room;
                        if (destinationRoom != null)
                        {
                            builder.Append(", to ");
                            builder.Append(destinationRoom.Short);
                        }

                        builder.Append(".\r\n");
                    }

					builder.AppendLine("\r\n");
				}

				Mud.SendEventMessage(Actor, EventMessageScope.Single, builder.ToString());
			}
		}
	}
}
