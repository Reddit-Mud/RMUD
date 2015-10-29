using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
	internal class Lock : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                BestScore("KEY",
                    BestScore("SUBJECT",
                        Sequence(
                            KeyWord("LOCK"),
                            MustMatch("@not here",
                                Object("SUBJECT", InScope)),
                            OptionalKeyWord("WITH"),
                            MustMatch("@not here",
                                Object("KEY", InScope, PreferHeld))))))
                .ID("StandardActions:Lock")
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "ACTOR", "SUBJECT", "KEY")
                .BeforeActing()
                .Perform("locked", "ACTOR", "SUBJECT", "KEY")
                .AfterActing();
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("not lockable", "I don't think the concept of 'locked' applies to that.");
            Core.StandardMessage("you lock", "You lock <the0>.");
            Core.StandardMessage("they lock", "^<the0> locks <the1> with <the2>.");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("lockable?", "[Item] : Can this item be locked?", "item");

            GlobalRules.Value<MudObject, bool>("lockable?").Do(item => false).Name("Things not lockable by default rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?", "actor", "item", "key");
            
            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to lock it.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsHolding(actor, key))
                .Name("Key must be held rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .When((actor, item, key) => !GlobalRules.ConsiderValueRule<bool>("lockable?", item))
                .Do((a, b, c) =>
                {
                    MudObject.SendMessage(a, "@not lockable");
                    return SharpRuleEngine.CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((a, b, c) => SharpRuleEngine.CheckResult.Allow)
                .Name("Default allow locking rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("locked", "[Actor, Item, Key] : Handle the actor locking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("locked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "@you lock", target);
                MudObject.SendExternalMessage(actor, "@they lock", actor, target, key);
                return SharpRuleEngine.PerformResult.Continue;
            });
        }
    }
}
