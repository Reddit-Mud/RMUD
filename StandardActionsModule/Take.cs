using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace StandardActionsModule
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
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can take?", actor, item) != CheckResult.Allow)
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

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            Core.StandardMessage("you take", "You take <the0>.");
            Core.StandardMessage("they take", "^<the0> takes <the1>.");
            Core.StandardMessage("cant take people", "You can't take people.");
            Core.StandardMessage("cant take portals", "You can't take portals.");
            Core.StandardMessage("cant take scenery", "That's a terrible idea.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can take?", "[Actor, Item] : Can the actor take the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("take", "[Actor, Item] : Handle the actor taking the item.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to take rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .When((actor, item) => actor is Container && (actor as Container).Contains(item, RelativeLocations.Held))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@already have that");
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
                    MudObject.SendMessage(actor, "@you take", target);
                    MudObject.SendExternalMessage(actor, "@they take", actor, target);
                    MudObject.Move(target, actor);
                    return PerformResult.Continue;
                })
                .Name("Default handle taken rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing is Actor)
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take people");
                    return CheckResult.Disallow;
                })
                .Name("Can't take people rule.");

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing.GetPropertyOrDefault<bool>("portal?", false))
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take portal");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing.GetBooleanProperty("scenery?"))
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take scenery");
                    return CheckResult.Disallow;
                })
                .Name("Can't take scenery rule.");
        }
    }

    public static class TakeRuleFactoryExtensions
    {
        public static RuleBuilder<MudObject, MudObject, CheckResult> CheckCanTake(this MudObject ThisObject)
        {
            return ThisObject.Check<MudObject, MudObject>("can take?").When((taker, obj) => System.Object.ReferenceEquals(obj, ThisObject));
        }

        public static RuleBuilder<MudObject, MudObject, PerformResult> PerformTake(this MudObject ThisObject)
        {
            return ThisObject.Perform<MudObject, MudObject>("take").When((taker, obj) => System.Object.ReferenceEquals(obj, ThisObject));
        }
    }
}
