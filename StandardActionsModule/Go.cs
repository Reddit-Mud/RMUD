﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace StandardActionsModule
{
	internal class Go : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                FirstOf(
                    Sequence(
                        KeyWord("GO"),
                        MustMatch("@unmatched cardinal", Cardinal("DIRECTION"))),
                    Cardinal("DIRECTION")))
                .Manual("Move between rooms. 'Go' is optional, a raw cardinal works just as well.")
                .ProceduralRule((match, actor) =>
                {
                    var direction = match["DIRECTION"] as Direction?;
                    var link = actor.Location.EnumerateObjects().FirstOrDefault(thing => thing.GetProperty<bool>("portal?") && thing.GetProperty<Direction>("link direction") == direction.Value);
                    match.Upsert("LINK", link);
                    return PerformResult.Continue;
                }, "lookup link rule")
                .ID("StandardActions:Go")
                .Check("can go?", "ACTOR", "LINK")
                .BeforeActing()
                .Perform("go", "ACTOR", "LINK")
                .AfterActing()
                .ProceduralRule((match, actor) =>
                {
                    Core.MarkLocaleForUpdate(actor);
                    Core.MarkLocaleForUpdate(match["LINK"] as MudObject);
                    return PerformResult.Continue;
                }, "Mark both sides of link for update rule");
		}

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            Core.StandardMessage("unmatched cardinal", "What way was that?");
            Core.StandardMessage("go to null link", "You can't go that way.");
            Core.StandardMessage("go to closed door", "The door is closed.");
            Core.StandardMessage("you went", "You went <s0>.");
            Core.StandardMessage("they went", "^<the0> went <s1>.");
            Core.StandardMessage("bad link", "Error - Link does not lead to a room.");
            Core.StandardMessage("they arrive", "^<the0> arrives <s1>.");
            Core.StandardMessage("first opening", "[first opening <the0>]");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can go?", "[Actor, Link] : Can the actor go through that link?", "actor", "link");

            GlobalRules.Check<MudObject, MudObject>("can go?")
                .When((actor, link) => link == null)
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "@go to null link");
                    return CheckResult.Disallow;
                })
                .Name("No link found rule.");

            GlobalRules.Check<MudObject, MudObject>("can go?")
                .When((actor, link) => link != null && link.GetProperty<bool>("openable?") && !link.GetProperty<bool>("open?"))
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "@first opening", link);
                    var tryOpen = Core.Try("StandardActions:Open", Core.ExecutingCommand.With("SUBJECT", link), actor);
                    if (tryOpen == PerformResult.Stop)
                        return CheckResult.Disallow;
                    return CheckResult.Continue;
                })
                .Name("Try opening a closed door first rule.");

            GlobalRules.Check<MudObject, MudObject>("can go?")
                .Do((actor, link) => CheckResult.Allow)
                .Name("Default can go rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("go", "[Actor, Link] : Handle the actor going through the link.", "actor", "link");

            GlobalRules.Perform<MudObject, MudObject>("go")
                .Do((actor, link) =>
                {
                    var direction = link.GetProperty<Direction>("link direction");
                    MudObject.SendMessage(actor, "@you went", direction.ToString().ToLower());
                    MudObject.SendExternalMessage(actor, "@they went", actor, direction.ToString().ToLower());
                    return PerformResult.Continue;
                })
                .Name("Report leaving rule.");

            GlobalRules.Perform<MudObject, MudObject>("go")
                .Do((actor, link) =>
                {
                    var destination = MudObject.GetObject(link.GetProperty<String>("link destination"));
                    if (destination == null)
                    {
                        MudObject.SendMessage(actor, "@bad link");
                        return PerformResult.Stop;
                    }
                    MudObject.Move(actor, destination);
                    return PerformResult.Continue;
                })
                .Name("Move through the link rule.");

            GlobalRules.Perform<MudObject, MudObject>("go")
                .Do((actor, link) =>
                {
                    var direction = link.GetProperty<Direction>("link direction");
                    var arriveMessage = Link.FromMessage(Link.Opposite(direction));
                    MudObject.SendExternalMessage(actor, "@they arrive", actor, arriveMessage);
                    return PerformResult.Continue;
                })
                .Name("Report arrival rule.");

            GlobalRules.Perform<MudObject, MudObject>("go")
                .When((actor, link) => actor.GetProperty<Client>("client") != null)
                .Do((actor, link) =>
                {
                    Core.EnqueuActorCommand(actor, "look", HelperExtensions.MakeDictionary("AUTO", true));
                    return PerformResult.Continue;
                })
                .Name("Players look after going rule.");
        }
    }

    public static class GoExtensions
    {
        public static RuleBuilder<MudObject, MudObject, CheckResult> CheckCanGo(this MudObject Object)
        {
            return Object.Check<MudObject, MudObject>("can go?").ThisOnly();
        }

        public static RuleBuilder<MudObject, MudObject, PerformResult> PerformGo(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject>("go");
        }
    }
}
