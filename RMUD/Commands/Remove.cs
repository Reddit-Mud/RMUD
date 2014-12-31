using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Remove : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("REMOVE"),
                    BestScore("OBJECT",
                        MustMatch("I couldn't figure out what you're trying to remove.",
                            Object("OBJECT", InScope, PreferWorn)))))
                .Manual("Expose your amazingly supple flesh.")
                .Check("can remove?", "ACTOR", "OBJECT")
                .Perform("removed", "ACTOR", "OBJECT");
        }

        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can remove?", "[Actor, Item] : Can the actor remove the item?");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("removed", "[Actor, Item] : Handle the actor removing the item.");

            GlobalRules.Check<MudObject, MudObject>("can remove?")
                .When((a, b) => !(a is Actor) || !(a as Actor).Contains(b, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "You'd have to be actually wearing that first.");
                    return CheckResult.Disallow;
                });
           
            GlobalRules.Check<MudObject, MudObject>("can remove?").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<MudObject, MudObject>("removed").Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "You remove <the0>.", target);
                    MudObject.SendExternalMessage(actor, "<a0> removes <a1>.", actor, target);
                    MudObject.Move(target, actor, RelativeLocations.Held);
                    return PerformResult.Continue;
                });
        }
    }
}
