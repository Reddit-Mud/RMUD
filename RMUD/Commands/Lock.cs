using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Lock : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                BestScore("KEY",
                    BestScore("SUBJECT",
                        Sequence(
                            KeyWord("LOCK"),
                            MustMatch("I couldn't figure out what you're trying to lock.",
                                Object("SUBJECT", InScope)),
                            OptionalKeyWord("WITH"),
                            MustMatch("I couldn't figure out what you're trying to lock that with.",
                                Object("KEY", InScope, PreferHeld))))))
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "ACTOR", "SUBJECT", "KEY")
                .BeforeActing()
                .Perform("locked", "ACTOR", "SUBJECT", "KEY")
                .AfterActing();
        }

        public void InitializeRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("lockable?", "[Item] : Can this item be locked?");

            GlobalRules.Value<MudObject, bool>("lockable?").Do(item => false).Name("Things not lockable by default rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?");
            
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
                    MudObject.SendMessage(a, "I don't think the concept of 'locked' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((a, b, c) => CheckResult.Allow)
                .Name("Default allow locking rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("locked", "[Actor, Item, Key] : Handle the actor locking the item with the key.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("locked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "You lock <the0>.", target);
                MudObject.SendExternalMessage(actor, "<a0> locks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
}
