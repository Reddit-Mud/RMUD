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
                new Sequence(
                    new KeyWord("UNLOCK", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource()),
                        "I couldn't figure out what you're trying to unlock."),
                    new KeyWord("WITH", true),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to unlock that with.")),
                new UnlockProcessor(),
                "Unlock something with something.",
                "SUBJECT-SCORE",
                "OBJECT-SCORE");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("on-unlocked-with", "Item based rulebook to handle the item being unlocked with something.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("on-unlocked-with").Do((actor, target, key) =>
            {
                Mud.SendMessage(actor, "You unlock <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> unlocks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }

    internal class UnlockProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["SUBJECT"] as MudObject;
            var key = Match.Arguments["OBJECT"] as MudObject;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            if (!Mud.ObjectContainsObject(Actor, key))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "You'd have to be holding " + key.Definite(Actor) + " for that to work.");
                return;
            }

            if (GlobalRules.ConsiderCheckRule("can-be-locked-with", target, Actor, target, key) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("on-unlocked-with", target, Actor, target, key);
        }
    }
}
