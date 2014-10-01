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

                if (location.IsLit)
                {
                    builder.Append(location.Long.Expand(Actor, location));
                    builder.Append("\r\n\r\n");

                    var visibleThings = new List<Thing>(location.Contents.Where(t => !Object.ReferenceEquals(t, Actor)));

                    for (int i = 0; i < visibleThings.Count; )
                    {
                        var localeDescribable = visibleThings[i] as LocaleDescriptionRules;
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
                        builder.Append("Also here: ");
                        builder.Append(String.Join(", ", visibleThings.Select(visibleThing =>
                        {
                            var subBuilder = new StringBuilder();
                            subBuilder.Append(visibleThing.Indefinite);

                            var container = visibleThing as IContainer;
                            if (container != null)
                            {
                                if ((container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                                {
                                    var subObjects = new List<Thing>();
                                    container.EnumerateObjects(RelativeLocations.On, (o,l) => 
                                    { 
                                        if (o is Thing)
                                            subObjects.Add(o as Thing); 
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

                        builder.Append("\r\n");
                    }
                }
                else
                {
                    builder.Append("It is too dark to see.\r\n\r\n");
                }

				Mud.SendMessage(Actor, MessageScope.Single, builder.ToString());
			}
		}
	}
}
