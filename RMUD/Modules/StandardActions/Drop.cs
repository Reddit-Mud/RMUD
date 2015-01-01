using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
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
                .Check("can drop?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("dropped", "ACTOR", "SUBJECT")
                .AfterActing();
		}

        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can drop?", "[Actor, Item] : Determine if the item can be dropped.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("dropped", "[Actor, Item] : Handle an item being dropped.");

            GlobalRules.Check<MudObject, MudObject>("can drop?")
                .First
                .When((actor, item) => !MudObject.ObjectContainsObject(actor, item))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "You aren't holding that.");
                    return CheckResult.Disallow;
                })
                .Name("Must be holding it to drop it rule.");

            GlobalRules.Check<MudObject, MudObject>("can drop?")
                .First
                .When((actor, item) => actor is Actor && (actor as Actor).Contains(item, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    if (GlobalRules.ConsiderCheckRule("can remove?", actor, item) == CheckResult.Allow)
                    {
                        GlobalRules.ConsiderPerformRule("remove", actor, item);
                        return CheckResult.Continue;
                    }
                    return CheckResult.Disallow;
                })
                .Name("Dropping worn items follows remove rules rule.");

            GlobalRules.Check<MudObject, MudObject>("can drop?").Do((a, b) => CheckResult.Allow).Name("Default can drop anything rule.");

            GlobalRules.Perform<MudObject, MudObject>("dropped").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "You drop <a0>.", target);
                MudObject.SendExternalMessage(actor, "<a0> drops <a1>.", actor, target);
                MudObject.Move(target, actor.Location);
                return PerformResult.Continue;
            }).Name("Default drop handler rule.");
        }
    }
}
