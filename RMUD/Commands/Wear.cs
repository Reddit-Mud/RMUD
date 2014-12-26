using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Wear : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("WEAR"),
                    BestScore("OBJECT",
                        MustMatch("I couldn't figure out what you're trying to wear.",
                            Object("OBJECT", InScope, PreferHeld)))),
                "Wear something.")
                .Manual("Cover your disgusting flesh.")
                .Check("can wear?", "OBJECT", "ACTOR", "OBJECT")
                .Perform("worn", "OBJECT", "ACTOR", "OBJECT");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, bool>("wearable?", "[Item => bool] : Can the item be worn?");
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can wear?", "[Actor, Item] : Can the actor wear the item?");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("worn", "[Actor, Item] : Handle the actor wearing the item.");

            GlobalRules.Value<MudObject, bool>("wearable?").Do(a => false).Name("Things unwearable by default rule.");

            GlobalRules.Check<MudObject, MudObject>("can wear?")
                .When((a, b) => !Mud.ObjectContainsObject(a, b))
                .Do((actor, item) =>
                {
                    Mud.SendMessage(actor, "You'd have to be holding that first.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<MudObject, MudObject>("can wear?")
                .When((a, b) => a is Actor && (a as Actor).LocationOf(b) == RelativeLocations.Worn)
                .Do((a, b) =>
                {
                    Mud.SendMessage(a, "You're already wearing that.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<MudObject, MudObject>("can wear?")
                .When((actor, item) => GlobalRules.ConsiderValueRule<bool>("wearable?", item, item) == false)
                .Do((actor, item) =>
                {
                    Mud.SendMessage(actor, "That isn't something that can be worn.");
                    return CheckResult.Disallow;
                })
                .Name("Can't wear unwearable things rule.");

            GlobalRules.Check<MudObject, MudObject>("can wear?").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<MudObject, MudObject>("worn").Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "You wear <the0>.", target);
                    Mud.SendExternalMessage(actor, "<a0> wears <a1>.", actor, target);
                    Mud.Move(target, actor, RelativeLocations.Worn);
                    return PerformResult.Continue;
                });
        }
    }
}
