using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
	internal class Take : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("GET"),
                        KeyWord("TAKE")),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can take?", actor, item) != CheckResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            })))))
                .Manual("Takes an item and adds it to your inventory.")
                .Check("can take?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("take", "ACTOR", "SUBJECT")
                .AfterActing()
                .MarkLocaleForUpdate();
		}

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can take?", "[Actor, Item] : Can the actor take the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("take", "[Actor, Item] : Handle the actor taking the item.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to take rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .When((actor, item) => actor is Container && (actor as Container).Contains(item, RelativeLocations.Held))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "You are already holding that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take what you're already holding rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .Last
                .Do((a, t) => CheckResult.Allow)
                .Name("Default allow taking rule.");

            GlobalRules.Perform<MudObject, MudObject>("take")
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "You take <a0>.", target);
                    MudObject.SendExternalMessage(actor, "<a0> takes <a1>.", actor, target);
                    MudObject.Move(target, actor);
                    return PerformResult.Continue;
                })
                .Name("Default handle taken rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing is Actor)
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "You can't take people.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take people rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing is Portal)
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "Portals cannot be taken.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing is Scenery)
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "That's a terrible idea.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take scenery rule.");
        }
    }
}
