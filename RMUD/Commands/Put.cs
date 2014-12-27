using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Put : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("PUT"),
                    BestScore("SUBJECT",
                        MustMatch("You don't seem to have that.",
                            Object("SUBJECT", InScope, PreferHeld))),
                    Optional(RelativeLocation("RELLOC")),
                    BestScore("OBJECT",
                        MustMatch("I can't see that here.",
                            Object("OBJECT", InScope, (actor, thing) =>
                                {
                                    //Prefer objects that are actually containers. No means curently to prefer
                                    //objects that actually support the relloc we matched previously.
                                    if (thing is Container) return MatchPreference.Likely;
                                    return MatchPreference.Plausible;
                                })))),
                "Put something on, in, under or behind something")
                .Manual("This commands allows you to put things on other things. While dropping just deposits the object into your current location, putting is much more specific.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.Arguments.ContainsKey("RELLOC"))
                    {
                        if (match.Arguments["OBJECT"] is Container)
                            match.Arguments.Upsert("RELLOC", (match.Arguments["OBJECT"] as Container).DefaultLocation);
                        else
                            match.Arguments.Upsert("RELLOC", RelativeLocations.On);
                    }
                    return PerformResult.Continue;
                }, "Supply default for optional relloc procedural rule.")
                .Check("can-put", "OBJECT", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .Perform("on-put", "OBJECT", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .MarkLocaleForUpdate();

        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("can-put", "[Actor, Item, Container, Location] : Determine if the actor can put the item in or on or under the container.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("on-put", "[Actor, Item, Container, Location] : Handle an actor putting the item in or on or under the container.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .Last
                .Do((a, b, c, d) => CheckResult.Allow)
                .Name("Allow putting as default rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .Do((actor, item, container, relloc) =>
                {
                    if (!(container is Container))
                    {
                        Mud.SendMessage(actor, "You can't put things " + Mud.GetRelativeLocationName(relloc) + " that.");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Can't put things in things that aren't containers rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .Do((actor, item, container, relloc) =>
                {
                    if (GlobalRules.ConsiderCheckRule("can-drop", item, actor, item) != CheckResult.Allow)
                        return CheckResult.Disallow;
                    return CheckResult.Continue;
                })
                .Name("Putting is dropping rule.");

            GlobalRules.Perform<MudObject, MudObject, MudObject, RelativeLocations>("on-put")
                .Do((actor, item, container, relloc) =>
                {
                    Mud.SendMessage(actor, String.Format("You put <the0> {0} <the1>.", Mud.GetRelativeLocationName(relloc)), item, container);
                    Mud.SendExternalMessage(actor, String.Format("<a0> puts <a1> {0} <a2>.", Mud.GetRelativeLocationName(relloc)), actor, item, container);
                    MudObject.Move(item, container, relloc);
                    return PerformResult.Continue;
                })
                .Name("Default putting things in things handler.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .Do((actor, item, container, relloc) =>
                {
                    var c = container as Container;
                    if (c == null || (c.LocationsSupported & relloc) != relloc)
                    {
                        Mud.SendMessage(actor, String.Format("You can't put something {0} that.", Mud.GetRelativeLocationName(relloc)));
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Check supported locations before putting rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .Do((actor, item, container, relloc) =>
                {
                    if (relloc == RelativeLocations.In && !GlobalRules.ConsiderValueRule<bool>("open?", container, container))
                    {
                        Mud.SendMessage(actor, "It seems to be closed.");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                })
                .Name("Can't put things in closed container rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .First
                .Do((actor, item, container, relloc) => GlobalRules.IsVisibleTo(actor, container))
                .Name("Container must be visible rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put")
                .First
                .Do((actor, item, container, relloc) => GlobalRules.IsHolding(actor, item))
                .Name("Must be holding item rule.");
        }
    }
}
