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
                new Or(
				    new KeyWord("LOOK", false),
                    new KeyWord("L", false)),
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

                builder.Append("\r\n");
				builder.Append(location.Short);
				builder.Append("\r\n");
				builder.Append(location.Long.Expand(Actor, location));
				builder.Append("\r\n\r\n");

                var visibleThings = new List<Thing>(location.Contents.Where(t => !Object.ReferenceEquals(t, Actor)));

                for (int i = 0; i < visibleThings.Count;)
                {
                    var localeDescribable = visibleThings[i] as ILocaleDescriptionRules;
                    if (localeDescribable != null)
                    {
                        visibleThings.RemoveAt(i);
                        builder.Append(localeDescribable.LocaleDescription.Expand(Actor, localeDescribable as MudObject));
                        builder.Append("\r\n\r\n");
                    }
                    else
                    {
                        ++i;
                    }
                }

                //Display objects in room
                if (visibleThings.Count > 0)
                {
                    builder.Append("Also here: " + String.Join(", ", visibleThings.Select(t => t.Indefinite)));
                    builder.Append("\r\n\r\n");
                }

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

				Mud.SendMessage(Actor, MessageScope.Single, builder.ToString());
			}
		}
	}
}
