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
                                Object("KEY", InScope, PreferHeld))))),
                "Lock something with something.")
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "SUBJECT", "ACTOR", "SUBJECT", "KEY")
                .Perform("locked", "SUBJECT", "ACTOR", "SUBJECT", "KEY");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("lockable?", "[Item] : Can this item be locked?");

            GlobalRules.Value<MudObject, bool>("lockable?").Do(item => false).Name("Things not lockable by default rule.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?");
            
            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => GlobalRules.IsVisibleTo(actor, item))
                .Name("Item must be visible to lock it.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => GlobalRules.IsHolding(actor, key))
                .Name("Key must be held rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .When((actor, item, key) => !GlobalRules.ConsiderValueRule<bool>("lockable?", item, item))
                .Do((a, b, c) =>
                {
                    Mud.SendMessage(a, "I don't think the concept of 'locked' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can lock?")
                .Do((a, b, c) => CheckResult.Allow)
                .Name("Default allow locking rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("locked", "[Actor, Item, Key] : Handle the actor locking the item with the key.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("locked").Do((actor, target, key) =>
            {
                Mud.SendMessage(actor, "You lock <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> locks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
}
