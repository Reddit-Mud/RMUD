using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
    internal class Look : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Or(
                    KeyWord("LOOK"),
                    KeyWord("L")))
                .Manual("Displays a description of your location, and lists what else is present there.")
                .ProceduralRule((match, actor) => GlobalRules.ConsiderPerformRule("describe locale", actor, actor.Location));
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe in locale", "[Actor, Item] : Generate a locale description for the item.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe locale", "[Actor, Room] : Generates a description of the locale.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .First
                .When((viewer, room) => room == null || !(room is Room))
                .Do((viewer, room) =>
                {
                    MudObject.SendMessage(viewer, "You aren't in any room.");
                    return PerformResult.Stop;
                })
                .Name("Can't describe the locale if there isn't one rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .First
                .Do((viewer, room) =>
                {
                    GlobalRules.ConsiderPerformRule("update", room);
                    return PerformResult.Continue;
                })
                .Name("Update room lighting before generating description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .First
                .Do((viewer, room) =>
                {
                    if (!String.IsNullOrEmpty(room.Short)) MudObject.SendMessage(viewer, room.Short);
                    return PerformResult.Continue;
                })
                .Name("Display room name rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .First
                .When((viewer, room) => (room as Room).Light == LightingLevel.Dark)
                .Do((viewer, room) =>
                {
                    MudObject.SendMessage(viewer, "It is too dark to see.");
                    return PerformResult.Stop;
                })
                .Name("Can't see in darkness rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .Do((viewer, room) =>
                {
                    GlobalRules.ConsiderPerformRule("describe", viewer, room);
                    return PerformResult.Continue;
                })
                .Name("Include describe rules in locale description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .Do((viewer, room) =>
                {
                    var visibleThings = (room as Room).EnumerateObjects(RelativeLocations.Contents).Where(t => !System.Object.ReferenceEquals(t, viewer));
                    var normalContents = new List<MudObject>();

                    foreach (var thing in visibleThings)
                    {
                        Core.BeginOutputQuery();
                        GlobalRules.ConsiderPerformRule("describe in locale", viewer, thing);
                        if (!Core.CheckOutputQuery()) normalContents.Add(thing);
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
                                    var subObjects = new List<MudObject>(container.EnumerateObjects(RelativeLocations.On));

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

                        MudObject.SendMessage(viewer, builder.ToString());
                    }

                    return PerformResult.Continue;
                })
                .Name("List contents of room rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .Do((viewer, room) =>
                {
                    if ((room as Room).EnumerateObjects(RelativeLocations.Links).Count() > 0)
                    {
                        MudObject.SendMessage(viewer, "Obvious exits:");

                        foreach (var link in (room as Room).EnumerateObjects<Link>(RelativeLocations.Links))
                        {
                            var builder = new StringBuilder();
                            builder.Append("  ^");
                            builder.Append(link.Direction.ToString());

                            if (link.Portal != null)
                            {
                                builder.Append(", through ");
                                builder.Append(link.Portal.Definite(viewer));
                            }

                            var destinationRoom = MudObject.GetObject(link.Destination) as Room;
                            if (destinationRoom != null)
                            {
                                builder.Append(", to ");
                                builder.Append(destinationRoom.Short);
                            }

                            MudObject.SendMessage(viewer, builder.ToString());
                        }
                    }

                    return PerformResult.Continue;
                })
                .Name("List exists in locale description rule.");
        }
    }
}
