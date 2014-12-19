using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Open : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("OPEN", false),
                    new ScoreGate(
                        new FailIfNoMatches(
                            new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                                (actor, openable) =>
                                {
                                    if (GlobalRules.ConsiderCheckRuleSilently("can-be-opened", openable, actor, openable) == CheckResult.Allow)
                                        return MatchPreference.Likely;
                                    return MatchPreference.Unlikely;
                                }),
                            "I don't see that here."),
                        "SUBJECT")),
                "Open something")
                .Check("can open?", "SUBJECT", "ACTOR", "SUBJECT")
                .Perform("opened", "SUBJECT", "ACTOR", "SUBJECT");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can open?", "[Actor, Item] : Can the actor open the item?");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("openable?", "[Item -> bool] : Is the item openable?");
            
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("opened", "[Actor, Item] : Handle the actor opening the item.");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("open?", "[Item -> bool] : Is the item open?");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .When((actor, item) => !GlobalRules.ConsiderValueRule<bool>("openable?", item, item))
                .Do((a, b) =>
                {
                    Mud.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't open the unopenable rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default go ahead and open it rule.");

            GlobalRules.Value<MudObject, bool>("open?").Do(a => false).Name("Things closed by default rule.");

            GlobalRules.Value<MudObject, bool>("openable?").Do(a => false).Name("Things unopenable by default rule.");

            GlobalRules.Perform<MudObject, MudObject>("opened").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You open <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> opens <a1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default report opening rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?").First.Do((actor, item) => GlobalRules.IsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
