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
                new Sequence(
                    new KeyWord("PUT", false),
                    new ScoreGate(
                        new FailIfNoMatches(
                            new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                            "You don't seem to have that."),
                        "SUBJECT"),
                    new Optional(new RelativeLocationMatcher("RELLOC")),
                    new ScoreGate(
                        new FailIfNoMatches(
                            new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                                {
                                    //Prefer objects that are actually containers. No means curently to prefer
                                    //objects that actually support the relloc we matched previously.
                                    if (Object is Container) return MatchPreference.Likely;
                                    return MatchPreference.Plausible;
                                }),
                            "I can't see that here."),
                       "OBJECT")),
                new PutProcessor(),
                "Put something on, in, under or behind something");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("can-put", "[Actor, Item, Container, Location] : Determine if the actor can put the item in or on or under the container.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("on-put", "[Actor, Item, Container, Location] : Handle an actor putting the item in or on or under the container.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put").First.Do((a, b, c, d) =>
            {
                if (c is Container) return CheckResult.Allow;
                return CheckResult.Disallow;
            }).Name("Default can't put things in things that aren't containers rule.");

            GlobalRules.Perform<MudObject, MudObject, MudObject, RelativeLocations>("on-put").Do((actor, item, container, relloc) =>
                {
                    Mud.SendMessage(actor, String.Format("You put <the0> {0} <the1>.", Mud.GetRelativeLocationName(relloc)), item, container);
                    Mud.SendExternalMessage(actor, String.Format("<a0> puts <a1> {0} <a2>.", Mud.GetRelativeLocationName(relloc)), actor, item, container);
                    MudObject.Move(item, container, relloc);
                    return PerformResult.Continue;
                }).Name("Default putting things in things handler.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put").First.Do((actor, item, container, relloc) =>
                {
                    if (relloc == RelativeLocations.In
                        && GlobalRules.ConsiderValueRule<bool>("openable", container, container)
                        && !GlobalRules.ConsiderValueRule<bool>("is-open", container, container))
                    {
                        Mud.SendMessage(actor, "It seems to be closed.");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                }).Name("Can't put things in closed container rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can-put").First.Do((actor, item, container, relloc) =>
                {
                    var c = container as Container;
                    if (c == null || (c.LocationsSupported & relloc) != relloc)
                    {
                        Mud.SendMessage(actor, String.Format("You can't put something {0} that.", Mud.GetRelativeLocationName(relloc)));
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                }).Name("Check supported locations before putting rule.");
        }
    }

	internal class PutProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;
            var container = Match.Arguments["OBJECT"] as MudObject;

            if (!Mud.IsVisibleTo(Actor, container))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            if (!Mud.ObjectContainsObject(Actor, target))
            {
                Mud.SendMessage(Actor, "You aren't holding that.");
                return;
            }
            
            RelativeLocations relloc = RelativeLocations.In;
            if (Match.Arguments.ContainsKey("RELLOC"))
                relloc = (Match.Arguments["RELLOC"] as RelativeLocations?).Value;
            else
            {
                if (container is Container) relloc = (container as Container).DefaultLocation;
            }
            
            if (GlobalRules.ConsiderCheckRule("can-drop", target, Actor, target) == CheckResult.Allow)
                if (GlobalRules.ConsiderCheckRule("can-put", container, Actor, target, container, relloc) == CheckResult.Allow)
                    GlobalRules.ConsiderPerformRule("on-put", container, Actor, target, container, relloc);

            Mud.MarkLocaleForUpdate(target);

        }
	}
}
