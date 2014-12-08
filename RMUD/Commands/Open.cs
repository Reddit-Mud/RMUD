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
                    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                             (actor, openable) =>
                             {
                                 if (GlobalRules.ConsiderActionRuleSilently("can-be-opened", openable, actor, openable) == RuleResult.Allow)
                                     return MatchPreference.Likely;
                                 return MatchPreference.Unlikely;
                             }),
                        "I don't see that here.")),
                new OpenProcessor(),
                "Open something",
                "SUBJECT-SCORE");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("can-open", "Item based rulebook to decide wether the item can be opened.");
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("openable", "[Item -> bool] Is the item openable?");
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("on-opened", "Item based rulebook to handle the item being opened.");
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("is-open", "[Item -> bool] Is the item open?");

            GlobalRules.AddActionRule<MudObject, MudObject>("can-open").Do((a, b) =>
            {
                Mud.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                return RuleResult.Disallow;
            });

            GlobalRules.AddValueRule<MudObject, bool>("is-open").Do(a => false);
            GlobalRules.AddValueRule<MudObject, bool>("openable").Do(a => false);

            GlobalRules.AddActionRule<MudObject, MudObject>("on-opened").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You open <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> opens <a1>.", actor, target);
                return RuleResult.Continue;
            });
        }
    }
	
	internal class OpenProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            if (GlobalRules.ConsiderActionRule("can-open", target, Actor, target) == RuleResult.Allow)
                GlobalRules.ConsiderActionRule("on-opened", target, Actor, target);
        }
	}
}
