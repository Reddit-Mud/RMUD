using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Look : CommandFactory, DeclaresRules
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

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("locale-description", "[Actor, Item] : Generate a locale description for the item.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe-locale", "[Actor, Room] : Generates a description of the locale.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .First
                .When((viewer, room) => room == null || !(room is Room))
                .Do((viewer, room) =>
                {
                    Mud.SendMessage(viewer, "You aren't in any room.");
                    return PerformResult.Stop;
                })
                .Name("Can't describe the locale if there isn't one rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .First
                .Do((viewer, room) =>
                {
                    room.HandleMarkedUpdate();
                    return PerformResult.Continue;
                })
                .Name("Update room lighting before generating description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .First
                .Do((viewer, room) =>
                {
                    Mud.SendMessage(viewer, room.Short);
                    return PerformResult.Continue;
                })
                .Name("Display room name rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .First
                .When((viewer, room) => (room as Room).AmbientLighting == LightingLevel.Dark)
                .Do((viewer, room) =>
                {
                    Mud.SendMessage(viewer, "It is too dark to see.");
                    return PerformResult.Stop;
                })
                .Name("Can't see in darkness rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .Do((viewer, room) =>
                {
                    GlobalRules.ConsiderPerformRule("describe", room, viewer, room);
                    return PerformResult.Continue;
                })
                .Name("Include describe rules in locale description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .Do((viewer, room) =>
                {
                    var visibleThings = (room as Room).EnumerateObjects(RelativeLocations.Contents).Where(t => !System.Object.ReferenceEquals(t, viewer));
                    var normalContents = new List<MudObject>();

                    foreach (var thing in visibleThings)
                    {
                        Mud.BeginOutputQuery();
                        GlobalRules.ConsiderPerformRule("locale-description", thing, viewer, thing);
                        if (!Mud.CheckOutputQuery()) normalContents.Add(thing);
                    }

                    if (normalContents.Count > 0)
                    {
                        var builder = new StringBuilder();
                        builder.Append("Also here: ");
                        builder.Append(String.Join(", ", normalContents.Select(thing =>
                        {
                            var subBuilder = new StringBuilder();
                            subBuilder.Append(thing.Indefinite(viewer));

                            var container = thing as Container;
                            if (container != null)
                            {
                                if ((container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                                {
                                    var subObjects = new List<MudObject>();
                                    container.EnumerateObjects(RelativeLocations.On, (o, l) =>
                                    {
                                        subObjects.Add(o);
                                        return EnumerateObjectsControl.Continue;
                                    });
                            
                                    if (subObjects.Count > 0)
                                    {
                                        subBuilder.Append(" (on which is ");
                                        subBuilder.Append(String.Join(", ", subObjects.Select(o => o.Indefinite(viewer))));
                                        subBuilder.Append(")");
                                    }
                                }
                            }

                            return subBuilder.ToString();
                        })));
                        
                        Mud.SendMessage(viewer, builder.ToString());
                    }

                    return PerformResult.Continue;
                })
                .Name("List contents of room rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe-locale")
                .Do((viewer, room) =>
                {
                    if ((room as Room).EnumerateObjects(RelativeLocations.Links).Count() > 0)
                    {
                        Mud.SendMessage(viewer, "Obvious exits:");

                        foreach (var link in (room as Room).EnumerateObjects<Link>(RelativeLocations.Links))
                        {
                            var builder = new StringBuilder();
                            builder.Append("  ");
                            builder.Append(Mud.CapFirst(link.Direction.ToString()));

                            if (link.Portal != null)
                            {
                                builder.Append(", through ");
                                builder.Append(link.Portal.Definite(viewer));
                            }

                            var destinationRoom = Mud.GetObject(link.Destination) as Room;
                            if (destinationRoom != null)
                            {
                                builder.Append(", to ");
                                builder.Append(destinationRoom.Short);
                            }

                            Mud.SendMessage(viewer, builder.ToString());
                        }
                    }

                    return PerformResult.Continue;
                })
                .Name("List exists in locale description rule.");
        }
    }

	public class LookProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            GlobalRules.ConsiderPerformRule("describe-locale", Actor.Location, Actor, Actor.Location);
		}
	}
}
