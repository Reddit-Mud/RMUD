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
                new Sequence(
                    new KeyWord("LOCK", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource()),
                        "I couldn't figure out what you're trying to lock."),
                    new KeyWord("WITH", true),
                    new FailIfNoMatches(
                        new ObjectMatcher("KEY", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to lock that with.")),
                new LockProcessor(),
                "Lock something with something.",
                "SUBJECT-SCORE",
                "KEY-SCORE");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can-be-locked-with", "Item based rulebook to decide wether the item can be locked using another item.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("on-locked-with", "Item based rulebook to handle the item being locked with something.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can-be-locked-with").Do((a, b, c) =>
            {
                Mud.SendMessage(a, "I don't think the concept of 'locked' applies to that.");
                return CheckResult.Disallow;
            });

            GlobalRules.Perform<MudObject, MudObject, MudObject>("on-locked-with").Do((actor, target, key) =>
            {
                Mud.SendMessage(actor, "You lock <the0>.", target);
                Mud.SendExternalMessage(actor, "<a0> locks <a1> with <a2>.", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
	
	internal class LockProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as MudObject;
			var key = Match.Arguments["KEY"] as MudObject;

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
                GlobalRules.ConsiderPerformRule("on-locked-with", target, Actor, target, key);
		}
	}
}
