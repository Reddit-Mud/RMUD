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
                Sequence(
                    KeyWord("DROP"),
                    BestScore("SUBJECT",
                        MustMatch("You don't seem to have that.",
                            Object("SUBJECT", InScope, PreferHeld)))))
                .Manual("Drop a held item. This can also be used to remove and drop a worn item.")
                .Check("can drop?", "SUBJECT", "ACTOR", "SUBJECT")
                .Perform("dropped", "SUBJECT", "ACTOR", "SUBJECT");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can drop?", "[Actor, Item] : Determine if the item can be dropped.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("dropped", "[Actor, Item] : Handle an item being dropped.");

            GlobalRules.Check<MudObject, MudObject>("can drop?")
                .First
                .When((actor, item) => !Mud.ObjectContainsObject(actor, item))
                .Do((actor, item) =>
                {
                    Mud.SendMessage(actor, "You aren't holding that.");
                    return CheckResult.Disallow;
                })
                .Name("Must be holding it to drop it rule.");

            GlobalRules.Check<MudObject, MudObject>("can drop?").Do((a, b) => CheckResult.Allow).Name("Default can drop anything rule.");

            GlobalRules.Perform<MudObject, MudObject>("dropped").Do((actor, target) =>
            {
                Mud.SendMessage(actor, "You drop <a0>.", target);
                Mud.SendExternalMessage(actor, "<a0> drops <a1>.", actor, target);
                MudObject.Move(target, actor.Location);
                return PerformResult.Continue;
            }).Name("Default drop handler rule.");
        }
    }
}
