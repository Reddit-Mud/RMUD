using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Take : CommandFactory, DeclaresRules
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
                .Perform("taken", "ACTOR", "SUBJECT")
                .MarkLocaleForUpdate();
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can take?", "[Actor, Item] : Can the actor take the item?");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("taken", "[Actor, Item] : Handle the actor taking the item.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .Do((actor, item) => GlobalRules.IsVisibleTo(actor, item))
                .Name("Item must be visible to take rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .When((actor, item) => actor is Container && (actor as Container).Contains(item, RelativeLocations.Held))
                .Do((actor, item) =>
                {
                    Mud.SendMessage(actor, "You are already holding that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take what you're already holding rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .Last
                .Do((a, t) => CheckResult.Allow)
                .Name("Default allow taking rule.");

            GlobalRules.Perform<MudObject, MudObject>("taken")
                .Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "You take <a0>.", target);
                    Mud.SendExternalMessage(actor, "<a0> takes <a1>.", actor, target);
                    MudObject.Move(target, actor);
                    return PerformResult.Continue;
                })
                .Name("Default handle taken rule.");
        }
    }
}
