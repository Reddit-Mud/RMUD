using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Unlock : CommandFactory, DeclaresRules
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
                .Perform("unlocked", "ACTOR", "ITEM", "KEY");
        }

        public void InitializeRules()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("unlocked", "[Actor, Item, Key] : Handle the actor unlocking the item with the key.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("unlocked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "You unlock <the0>.", target);
                MudObject.SendExternalMessage(actor, "<a0> unlocks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
}
