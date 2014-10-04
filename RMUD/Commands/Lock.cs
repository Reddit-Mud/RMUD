using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Lock : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("LOCK", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                            (actor, matchable) =>
                            {
                                if (matchable is LockableRules && !(matchable as LockableRules).Locked)
                                    return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            }),
                        "I couldn't figure out what you're trying to lock.\r\n"),
                    new KeyWord("WITH", true),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to lock that with.\r\n")),
                new LockProcessor(),
                "Lock someMudObject with someMudObject",
                "SUBJECT-SCORE",
                "OBJECT-SCORE");
        }
	}
	
	internal class LockProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as LockableRules;
			var key = Match.Arguments["OBJECT"] as MudObject;

			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "I don't think the concept of 'locked' applies to that.\r\n");
				return;
			}

			if (!Mud.IsVisibleTo(Actor, target as MudObject))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                return;
            }

			if (!Mud.ObjectContainsObject(Actor, key))
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "You'd have to be holding " + key.Definite + " for that to work.\r\n");
				return;
			}

			if (target.Locked)
			{
				Mud.SendMessage(Actor, "It's already locked.\r\n");
				return;
			}

            var checkRule = target.CheckLock(Actor, key);
            if (checkRule.Allowed)
            {
                if (target.HandleLock(Actor, key) == RuleHandlerFollowUp.Continue)
                {
                    var MudObject = target as MudObject;
                    if (MudObject != null)
                    {
                        Mud.SendMessage(Actor, "You lock " + MudObject.Definite + ".\r\n");
                        Mud.SendExternalMessage(Actor, Actor.Short + " locks " + MudObject.Indefinite + " with " + key.Indefinite + ".\r\n");
                    }
                }
            }
            else
            {
                Mud.SendMessage(Actor, checkRule.ReasonDisallowed + "\r\n");
            }
		}
	}

}
