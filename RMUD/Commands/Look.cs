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

	internal class LookProcessor : CommandProcessor
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

                if (location.IsLit)
                {
                    builder.Append(location.Long.Expand(Actor, location));
                    builder.Append("\r\n\r\n");

                    var visibleMudObjects = new List<MudObject>(location.Contents.Where(t => !Object.ReferenceEquals(t, Actor)));

                    for (int i = 0; i < visibleMudObjects.Count; )
                    {
                        var localeDescribable = visibleMudObjects[i] as LocaleDescriptionRules;
                        if (localeDescribable != null)
                        {
                            visibleMudObjects.RemoveAt(i);
                            builder.Append(localeDescribable.LocaleDescription.Expand(Actor, localeDescribable as MudObject));
                            builder.Append("\r\n\r\n");
                        }
                        else
                        {
                            ++i;
                        }
                    }

                    //Display objects in room
                    if (visibleMudObjects.Count > 0)
                    {
                        builder.Append("Also here: ");
                        builder.Append(String.Join(", ", visibleMudObjects.Select(visibleMudObject =>
                        {
                            var subBuilder = new StringBuilder();
                            subBuilder.Append(visibleMudObject.Indefinite);

                            var container = visibleMudObject as Container;
                            if (container != null)
                            {
                                if ((container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                                {
                                    var subObjects = new List<MudObject>();
                                    container.EnumerateObjects(RelativeLocations.On, (o,l) => 
                                    { 
                                        if (o is MudObject)
                                            subObjects.Add(o as MudObject); 
                                        return EnumerateObjectsControl.Continue;
                                    });
                                    if (subObjects.Count > 0)
                                    {
                                        subBuilder.Append(" (on which is ");
                                        subBuilder.Append(String.Join(", ", subObjects.Select(o => o.Indefinite)));
                                        subBuilder.Append(")");
                                    }
                                }
                            }

                            return subBuilder.ToString();
                        })));

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

                            if (link.Portal != null)
                            {
                                builder.Append(", through ");
                                builder.Append(link.Portal.Indefinite);
                            }

                            var destinationRoom = Mud.GetObject(link.Destination) as Room;
                            if (destinationRoom != null)
                            {
                                builder.Append(", to ");
                                builder.Append(destinationRoom.Short);
                            }

                            builder.Append(".\r\n");
                        }

                        builder.Append("\r\n");
                    }
                }
                else
                {
                    builder.Append("It is too dark to see.\r\n\r\n");
                }

				Mud.SendMessage(Actor, builder.ToString());
			}
		}
	}
}
