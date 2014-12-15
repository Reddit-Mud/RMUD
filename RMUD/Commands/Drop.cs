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
                    new ScoreGate(
                        new FailIfNoMatches(
					        new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                            "I don't know what object you're talking about."),
                        "SUBJECT")),
				new DropProcessor(),
				"Drop something");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-drop", "[Actor, Item] : Determine if the item can be dropped.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("on-dropped", "[Actor, Item] : Handle an item being dropped.");

            GlobalRules.Check<MudObject, MudObject>("can-drop").Do((a, b) => CheckResult.Allow).Name("Default can drop anything");

            GlobalRules.Perform<MudObject, MudObject>("on-dropped").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You drop <a0>.", target);
                Mud.SendExternalMessage(actor, "<a0> drops <a1>.", actor, target);
                MudObject.Move(target, actor.Location);
                return PerformResult.Continue;
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

            if (GlobalRules.ConsiderCheckRule("can-drop", target, Actor, target) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("on-dropped", target, Actor, target);

            Mud.MarkLocaleForUpdate(target);
        }
	}
}
