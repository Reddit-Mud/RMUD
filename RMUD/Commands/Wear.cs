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
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-be-worn", "Item based rulebook to decide whether the item is wearable.");
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-wear", "Actor based rulebook to decide if the actor can wear something.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("on-donned", "Item based rulebook to handle the item being worn.");

            GlobalRules.Check<MudObject, MudObject>("can-be-worn").Do((a, b) =>
                {
                    Mud.SendMessage(a, "That isn't something you can wear.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<MudObject, MudObject>("can-wear").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<MudObject, MudObject>("on-donned").Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "You wear <the0>.", target);
                    Mud.SendExternalMessage(actor, "<a0> wears <a1>.", actor, target);
                    Mud.Move(target, actor, RelativeLocations.Worn);
                    return PerformResult.Continue;
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

            var canBeWorn = GlobalRules.ConsiderCheckRule("can-be-worn", target, Actor, target);
            if (canBeWorn == CheckResult.Allow) canBeWorn = GlobalRules.ConsiderCheckRule("can-wear", Actor, Actor, target);

            if (canBeWorn == CheckResult.Allow)
            {
                GlobalRules.ConsiderPerformRule("on-donned", target, Actor, target);
            }
		}
	}
}
