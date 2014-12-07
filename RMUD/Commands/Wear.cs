using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Wear : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("WEAR", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to wear.")),
                new WearProcessor(),
                "Wear something.",
                "OBJECT-SCORE");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("can-be-worn", "Item based rulebook to decide whether the item is wearable.");
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("can-wear", "Actor based rulebook to decide if the actor can wear something.");
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("on-donned", "Item based rulebook to handle the item being worn.");

            GlobalRules.AddActionRule<MudObject, MudObject>("can-be-worn").Do((a, b) =>
                {
                    Mud.SendMessage(a, "That isn't something you can wear.");
                    return RuleResult.Disallow;
                });

            GlobalRules.AddActionRule<MudObject, MudObject>("can-wear").Do((a, b) => RuleResult.Allow);

            GlobalRules.AddActionRule<MudObject, MudObject>("on-donned").Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "You wear <the0>.", target);
                    Mud.SendExternalMessage(actor, "<a0> wears <a1>.", actor, target);
                    Mud.Move(target, actor, RelativeLocations.Worn);
                    return RuleResult.Continue;
                });
        }
    }
	
	internal class WearProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["OBJECT"] as MudObject;

            if (!CommandHelper.CheckHolding(Actor, target)) return;

            if (Actor.LocationOf(target) == RelativeLocations.Worn)
            {
                Mud.SendMessage(Actor, "You're already wearing that.");
                return;
            }

            var canBeWorn = GlobalRules.ConsiderActionRule("can-be-worn", target, Actor, target);
            if (canBeWorn == RuleResult.Allow) canBeWorn = GlobalRules.ConsiderActionRule("can-wear", Actor, Actor, target);

            if (canBeWorn == RuleResult.Allow)
            {
                GlobalRules.ConsiderActionRule("on-donned", target, Actor, target);
            }
		}
	}
}
