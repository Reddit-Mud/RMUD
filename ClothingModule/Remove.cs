using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace ClothingModule
{
	internal class Remove : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("REMOVE"),
                    BestScore("OBJECT",
                        MustMatch("@clothing remove what",
                            Object("OBJECT", InScope, PreferWorn)))))
                .Manual("Expose your amazingly supple flesh.")
                .Check("can remove?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("removed", "ACTOR", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can remove?", "[Actor, Item] : Can the actor remove the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("removed", "[Actor, Item] : Handle the actor removing the item.", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can remove?")
                .When((a, b) => !(a is Actor) || !(a as Actor).Contains(b, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@clothing not wearing");
                    return CheckResult.Disallow;
                });
           
            GlobalRules.Check<MudObject, MudObject>("can remove?").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<MudObject, MudObject>("removed").Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@clothing you remove", target);
                    MudObject.SendExternalMessage(actor, "@clothing they remove", actor, target);
                    MudObject.Move(target, actor, RelativeLocations.Held);
                    return PerformResult.Continue;
                });
        }
    }
}
