using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Wear : CommandFactory, ActionRules
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

        public void CreateGlobalRules()
        {
            GlobalRuleBooks.DeclareRuleBook<MudObject, MudObject>("can-be-worn");
            GlobalRuleBooks.DeclareRuleBook<MudObject, MudObject>("can-wear");
            GlobalRuleBooks.DeclareRuleBook<MudObject, MudObject>("on-donned");
            GlobalRuleBooks.DeclareRuleBook<MudObject, MudObject>("on-dons");

            GlobalRuleBooks.AddRule<MudObject, MudObject>("can-be-worn").Do((a, b) =>
                {
                    Mud.SendMessage(a, "That isn't something you can wear.");
                    return RuleResult.Disallow;
                });

            GlobalRuleBooks.AddRule<MudObject, MudObject>("can-wear").Do((a, b) => RuleResult.Allow);

            GlobalRuleBooks.AddRule<MudObject, MudObject>("on-donned").Do((actor, target) =>
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

			if (!Mud.ObjectContainsObject(Actor, target))
			{
				Mud.SendMessage(Actor, "You'd have to be holding <the0> for that to work.", target);
				return;
			}

            if (Actor.LocationOf(target) == RelativeLocations.Worn)
            {
                Mud.SendMessage(Actor, "You're already wearing that.");
                return;
            }

            var canBeWorn = GlobalRuleBooks.ConsiderRuleFamily("can-be-worn", target, Actor, target);
            if (canBeWorn == RuleResult.Allow) canBeWorn = GlobalRuleBooks.ConsiderRuleFamily("can-wear", Actor, Actor, target);

            if (canBeWorn == RuleResult.Allow)
            {
                GlobalRuleBooks.ConsiderRuleFamily("on-donned", target, Actor, target);
                GlobalRuleBooks.ConsiderRuleFamily("on-dons", Actor, Actor, target);
            }
		}
	}
}
