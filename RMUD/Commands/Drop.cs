using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Drop : CommandFactory, DeclaresRules
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("DROP", false),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I don't know what object you're talking about.")),
				new DropProcessor(),
				"Drop something",
                "SUBJECT-SCORE");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("can-drop", "[Actor, Item] : Determine if the item can be dropped.");
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("on-dropped", "[Actor, Item] : Handle an item being dropped.");

            GlobalRules.AddActionRule<MudObject, MudObject>("can-drop").Do((a, b) => RuleResult.Allow).Name("Default can drop anything");

            GlobalRules.AddActionRule<MudObject, MudObject>("on-dropped").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You drop <a0>.", target);
                Mud.SendExternalMessage(actor, "<a0> drops <a1>.", actor, target);
                MudObject.Move(target, actor.Location);
                return RuleResult.Continue;
            }).Name("Default drop handler");
        }
    }

	internal class DropProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;

            if (!Mud.ObjectContainsObject(Actor, target))
            {
                Mud.SendMessage(Actor, "You aren't holding that.");
                return;
            }

            if (GlobalRules.ConsiderActionRule("can-drop", target, Actor, target) == RuleResult.Allow)
                GlobalRules.ConsiderActionRule("on-dropped", target, Actor, target);

            Mud.MarkLocaleForUpdate(target);
        }
	}
}
