using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
	internal class Pull : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("PULL"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can pull?", actor, item) != SharpRuleEngine.CheckResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            })))))
                .ID("StandardActions:Pull")
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
                        MudObject.SendMessage(a, "@does nothing");
                        return SharpRuleEngine.CheckResult.Disallow;
                    })
                .Name("Default disallow pulling rule.");

            GlobalRules.Perform<MudObject, MudObject>("pull")
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@nothing happens");
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Default handle pulling rule.");

            GlobalRules.Check<MudObject, Actor>("can pull?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@unappreciated", thing);
                    return SharpRuleEngine.CheckResult.Disallow;
                })
                .Name("Can't pull people rule.");
        }
    }
}
