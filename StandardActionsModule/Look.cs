using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
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
                .ProceduralRule((match, actor) => Core.GlobalRules.ConsiderPerformRule("describe locale", actor, actor.Location));
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("nowhere", "You aren't anywhere.");
            Core.StandardMessage("dark", "It's too dark to see.");
            Core.StandardMessage("also here", "Also here: <l0>.");
            Core.StandardMessage("on which", "(on which is <l0>)");
            Core.StandardMessage("obvious exits", "Obvious exits:");
            Core.StandardMessage("through", "through <the0>");
            Core.StandardMessage("to", "to <the0>");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe in locale", "[Actor, Item] : Generate a locale description for the item.", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe locale", "[Actor, Room] : Generates a description of the locale.", "actor", "room");

            GlobalRules.Perform<MudObject, MudObject>("describe locale")
                .First
                .When((viewer, room) => room == null || !(room is Room))
                .Do((viewer, room) =>
                {
                    MudObject.SendMessage(viewer, "@nowhere");
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
                    MudObject.SendMessage(viewer, "@dark");
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

            var describingLocale = false;

            GlobalRules.Value<Actor, Container, String, String>("printed name")
                .When((viewer, container, article) =>
                    {
                        return describingLocale && (container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On;
                    })
                .Do((viewer, container, article) =>
                    {
                        var subObjects = new List<MudObject>(container.EnumerateObjects(RelativeLocations.On));

                        if (subObjects.Count > 0)
                            return container.Short + " " + Core.FormatMessage(viewer, Core.GetMessage("on which"), subObjects);
                        else
                            return container.Short;
                    })
                    .Name("List contents of container after name when describing locale rule");

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
                        describingLocale = true;
                        MudObject.SendMessage(viewer, "@also here", normalContents);
                        describingLocale = false;
                    }

                    return PerformResult.Continue;
                })
                .Name("List contents of room rule.");

            GlobalRules.Perform<Actor, MudObject>("describe locale")
                .Last
                .Do((viewer, room) =>
                {
                    if ((room as Room).EnumerateObjects(RelativeLocations.Links).Count() > 0)
                    {
                        MudObject.SendMessage(viewer, "@obvious exits");

                        foreach (var link in (room as Room).EnumerateObjects<MudObject>(RelativeLocations.Links))
                        {
                            var builder = new StringBuilder();
                            builder.Append("  ^");
                            builder.Append(link.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE).ToString());

                            if (!link.GetPropertyOrDefault<bool>("link anonymous?", false))
                                builder.Append(" " + Core.FormatMessage(viewer, Core.GetMessage("through"), link));

                            var destinationRoom = MudObject.GetObject(link.GetProperty<String>("link destination")) as Room;
                            if (destinationRoom != null)
                                builder.Append(" " + Core.FormatMessage(viewer, Core.GetMessage("to"), destinationRoom));

                            MudObject.SendMessage(viewer, builder.ToString());
                        }
                    }

                    return PerformResult.Continue;
                })
                .Name("List exits in locale description rule.");
        }
    }
}
