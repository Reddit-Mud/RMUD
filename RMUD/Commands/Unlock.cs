using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Unlock : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("UNLOCK", false),
				    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                            (actor, matchable) =>
                            {
                                if (matchable is LockableRules && (matchable as LockableRules).Locked)
                                    return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            }),
                        "I couldn't figure out what you're trying to unlock."),
                    new KeyWord("WITH", true),
                    new FailIfNoMatches(
					    new ObjectMatcher("OBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to unlock that with.")),
				new UnlockProcessor(),
				"Unlock something with something",
                "SUBJECT-SCORE",
                "OBJECT-SCORE");
		}
	}
	
	internal class UnlockProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as LockableRules;
			var key = Match.Arguments["OBJECT"] as MudObject;
			
			if (target == null)
			{
				if (Actor.ConnectedClient != null) 
					Mud.SendMessage(Actor, "I don't think the concept of 'locked' applies to that.");
				return;
			}

            if (!Mud.IsVisibleTo(Actor, target as MudObject))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

			if (!Mud.ObjectContainsObject(Actor, key))
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "You'd have to be holding <the0> for that to work.", key);
				return;
			}

			if (!target.Locked)
			{
				Mud.SendMessage(Actor, "It's not locked.");
				return;
			}

            var checkRule = target.CheckUnlock(Actor, key);
            if (checkRule.Allowed)
            {
                if (target.HandleUnlock(Actor, key) == RuleHandlerFollowUp.Continue)
                {
                    var MudObject = target as MudObject;
                    if (MudObject != null)
                    {
                        Mud.SendMessage(Actor, "You unlock <a0>.", MudObject);
                        Mud.SendExternalMessage(Actor, "<a0> unlocks <a1> with <a2>.", Actor, MudObject, key);
                    }
                }
            }
            else
            {
                Mud.SendMessage(Actor, checkRule.ReasonDisallowed);
            }
		}
	}
}
