using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
	internal class Pull : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("PULL"),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can pull?", actor, item) != CheckResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            })))))
                .Manual("Pull an item. By default, this does nothing.")
                .Check("can pull?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("pull", "ACTOR", "SUBJECT")
                .AfterActing()
                .MarkLocaleForUpdate();
		}

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can pull?", "[Actor, Item] : Can the actor pull the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("pull", "[Actor, Item] : Handle the actor pulling the item.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can pull?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to pull rule.");

            GlobalRules.Check<MudObject, MudObject>("can pull?")
                .Last
                .Do((a, t) => 
                    {
                        MudObject.SendMessage(a, "Pulling <the0> doesn't seem to do anything.", t);
                        return CheckResult.Disallow;
                    })
                .Name("Default disallow pulling rule.");

            GlobalRules.Perform<MudObject, MudObject>("pull")
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "Nothing happens.");
                    return PerformResult.Continue;
                })
                .Name("Default handle pulling rule.");

            GlobalRules.Check<MudObject, Actor>("can pull?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "I don't think <the0> would appreciate that.", thing);
                    return CheckResult.Disallow;
                })
                .Name("Can't pull people rule.");
        }
    }
}
