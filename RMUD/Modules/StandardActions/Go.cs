using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
	internal class Go : CommandFactory, DeclaresRules
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                FirstOf(
                    Sequence(
                        KeyWord("GO"),
                        MustMatch("What way was that?", Cardinal("DIRECTION"))),
                    Cardinal("DIRECTION")))
                .Manual("Move between rooms. 'Go' is optional, a raw cardinal works just as well.")
                .ProceduralRule((match, actor) =>
                {
                    var direction = match["DIRECTION"] as Direction?;
                    var location = actor.Location as Room;
                    var link = location.EnumerateObjects().FirstOrDefault(thing => thing is Link && (thing as Link).Direction == direction.Value);
                    match.Upsert("LINK", link);
                    return PerformResult.Continue;
                }, "lookup link rule")
                .Check("can go?", "ACTOR", "LINK")
                .BeforeActing()
                .Perform("go", "ACTOR", "LINK")
                .AfterActing()
                .ProceduralRule((match, actor) =>
                {
                    MudObject.MarkLocaleForUpdate(actor);
                    MudObject.MarkLocaleForUpdate(match["LINK"] as MudObject);
                    return PerformResult.Continue;
                }, "Mark both sides of link for update rule");
		}

        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, Link>("can go?", "[Actor, Link] : Can the actor go through that link?");

            GlobalRules.Check<MudObject, Link>("can go?")
                .When((actor, link) => link == null)
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "You can't go that way.");
                    return CheckResult.Disallow;
                })
                .Name("No link found rule.");

            GlobalRules.Check<MudObject, Link>("can go?")
                .When((actor, link) => (link.Portal != null) && !GlobalRules.ConsiderValueRule<bool>("open?", link.Portal))
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "The door is closed.");
                    return CheckResult.Disallow;
                })
                .Name("Can't go through closed door rule.");

            GlobalRules.Check<MudObject, Link>("can go?")
                .Do((actor, link) => CheckResult.Allow)
                .Name("Default can go rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, Link>("go", "[Actor, Link] : Handle the actor going through the link.");

            GlobalRules.Perform<MudObject, Link>("go")
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "You went " + link.Direction.ToString().ToLower() + ".");
                    MudObject.SendExternalMessage(actor, "^<the0> went " + link.Direction.ToString().ToLower() + ".", actor);
                    return PerformResult.Continue;
                })
                .Name("Report leaving rule.");

            GlobalRules.Perform<MudObject, Link>("go")
                .Do((actor, link) =>
                {
                    var destination = Core.GetObject(link.Destination, s => MudObject.SendMessage(actor, s)) as Room;
                    if (destination == null)
                    {
                        MudObject.SendMessage(actor, "Error - Link does not lead to a room.");
                        return PerformResult.Stop;
                    }
                    MudObject.Move(actor, destination);
                    return PerformResult.Continue;
                })
                .Name("Move through the link rule.");

            GlobalRules.Perform<MudObject, Link>("go")
                .Do((actor, link) =>
                {
                    var arriveMessage = Link.FromMessage(Link.Opposite(link.Direction));
                    MudObject.SendExternalMessage(actor, "^<the0> arrives " + arriveMessage + ".", actor);
                    return PerformResult.Continue;
                })
                .Name("Report arrival rule.");

            GlobalRules.Perform<MudObject, Link>("go")
                .When((actor, link) => actor is Player && (actor as Player).ConnectedClient != null)
                .Do((actor, link) =>
                {
                    Core.EnqueuClientCommand((actor as Player).ConnectedClient, "look");
                    return PerformResult.Continue;
                })
                .Name("Players look after going rule.");
        }
    }
}
