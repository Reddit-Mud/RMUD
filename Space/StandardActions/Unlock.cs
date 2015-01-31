using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
    internal class Unlock : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("UNLOCK"),
                    BestScore("ITEM",
                        MustMatch("I couldn't figure out what you're trying to unlock.",
                            Object("ITEM", InScope))),
                    OptionalKeyWord("WITH"),
                    BestScore("KEY",
                        MustMatch("I couldn't figure out what you're trying to unlock that with.",
                            Object("KEY", InScope, PreferHeld)))))
                .Manual("Use the KEY to unlock the ITEM.")
                .Check("can lock?", "ACTOR", "ITEM", "KEY")
                .BeforeActing()
                .Perform("unlocked", "ACTOR", "ITEM", "KEY")
                .AfterActing();
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("unlocked", "[Actor, Item, Key] : Handle the actor unlocking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("unlocked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "You unlock <the0>.", target);
                MudObject.SendExternalMessage(actor, "<a0> unlocks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
}
