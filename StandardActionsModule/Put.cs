using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace StandardActionsModule
{
	internal class Put : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("PUT"),
                    BestScore("SUBJECT",
                        MustMatch("@dont have that",
                            Object("SUBJECT", InScope, PreferHeld))),
                    Optional(RelativeLocation("RELLOC")),
                    BestScore("OBJECT",
                        MustMatch("@not here",
                            Object("OBJECT", InScope, (actor, thing) =>
                                {
                                    //Prefer objects that are actually containers. No means curently to prefer
                                    //objects that actually support the relloc we matched previously.
                                    if (thing is Container) return MatchPreference.Likely;
                                    return MatchPreference.Plausible;
                                })))))
                .Manual("This commands allows you to put things on other things. While dropping just deposits the object into your current location, putting is much more specific.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("RELLOC"))
                    {
                        if (match["OBJECT"] is Container)
                            match.Upsert("RELLOC", (match["OBJECT"] as Container).DefaultLocation);
                        else
                            match.Upsert("RELLOC", RelativeLocations.On);
                    }
                    return SharpRuleEngine.PerformResult.Continue;
                }, "Supply default for optional relloc procedural rule.")
                .Check("can put?", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .BeforeActing()
                .Perform("put", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .AfterActing()
                .MarkLocaleForUpdate();

        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            Core.StandardMessage("cant put relloc", "You can't put things <s0> that.");
            Core.StandardMessage("you put", "You put <the0> <s1> <the2>.");
            Core.StandardMessage("they put", "^<the0> puts <the1> <s2> <the3>.");
                
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("can put?", "[Actor, Item, Container, Location] : Determine if the actor can put the item in or on or under the container.", "actor", "item", "container", "relloc");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("put", "[Actor, Item, Container, Location] : Handle an actor putting the item in or on or under the container.", "actor", "item", "container", "relloc");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Last
                .Do((a, b, c, d) => CheckResult.Allow)
                .Name("Allow putting as default rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (!(container is Container))
                    {
                        MudObject.SendMessage(actor, "@cant put relloc", Relloc.GetRelativeLocationName(relloc));
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Can't put things in things that aren't containers rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (GlobalRules.ConsiderCheckRule("can drop?", actor, item) != CheckResult.Allow)
                        return CheckResult.Disallow;
                    return CheckResult.Continue;
                })
                .Name("Putting is dropping rule.");

            GlobalRules.Perform<MudObject, MudObject, MudObject, RelativeLocations>("put")
                .Do((actor, item, container, relloc) =>
                {
                    MudObject.SendMessage(actor, "@you put", item, Relloc.GetRelativeLocationName(relloc), container);
                    MudObject.SendExternalMessage(actor, "@they put", actor, item, Relloc.GetRelativeLocationName(relloc), container);
                    MudObject.Move(item, container, relloc);
                    return PerformResult.Continue;
                })
                .Name("Default putting things in things handler.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    var c = container as Container;
                    if (c == null || (c.LocationsSupported & relloc) != relloc)
                    {
                        MudObject.SendMessage(actor, "@cant put relloc", Relloc.GetRelativeLocationName(relloc));
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Check supported locations before putting rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (relloc == RelativeLocations.In && !container.GetBooleanProperty("open?"))
                    {
                        MudObject.SendMessage(actor, "@is closed error", container);
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                })
                .Name("Can't put things in closed container rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .First
                .Do((actor, item, container, relloc) => MudObject.CheckIsVisibleTo(actor, container))
                .Name("Container must be visible rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .First
                .Do((actor, item, container, relloc) => MudObject.CheckIsHolding(actor, item))
                .Name("Must be holding item rule.");
        }
    }
}
