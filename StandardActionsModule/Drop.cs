using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
	internal class Drop : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("DROP"),
                    BestScore("SUBJECT",
                        MustMatch("@dont have that",
                            Object("SUBJECT", InScope, PreferHeld)))))
                .Manual("Drop a held item. This can also be used to remove and drop a worn item.")
                .Check("can drop?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("drop", "ACTOR", "SUBJECT")
                .AfterActing();
		}

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("you drop", "You drop <the0>.");
            Core.StandardMessage("they drop", "^<the0> drops <a1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can drop?", "[Actor, Item] : Determine if the item can be dropped.", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("drop", "[Actor, Item] : Handle an item being dropped.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can drop?")
                .First
                .When((actor, item) => !MudObject.ObjectContainsObject(actor, item))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@dont have that");
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

            GlobalRules.Perform<MudObject, MudObject>("drop").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "@you drop", target);
                MudObject.SendExternalMessage(actor, "@they drop", actor, target);
                MudObject.Move(target, actor.Location);
                return PerformResult.Continue;
            }).Name("Default drop handler rule.");
        }
    }

    public static class DropExtensions
    {
        public static RuleBuilder<MudObject, MudObject, PerformResult> PerformDrop(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject>("drop").ThisOnly(1);
        }

        public static RuleBuilder<MudObject, MudObject, CheckResult> CheckCanDrop(this MudObject Object)
        {
            return Object.Check<MudObject, MudObject>("can drop?").ThisOnly(1);
        }
    }
}
