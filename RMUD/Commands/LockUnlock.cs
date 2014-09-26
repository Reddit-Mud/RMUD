using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class LockUnlock : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("LOCK", false),
					new ObjectMatcher("SUBJECT", new InScopeObjectSource(), 
                        (actor, matchable) => {
                            if (matchable is ILockableRules && !(matchable as ILockableRules).Locked)
                                return 1;
                            return -1;
                        }, "SUBJECTSCORE"),
					new KeyWord("WITH", true),
					new ObjectMatcher("OBJECT", new InScopeObjectSource(), 
                        ObjectMatcher.PreferHeld, "OBJECTSCORE")),
				new LockProcessor(),
				"Lock something with something",
                "SUBJECTSCORE",
                "OBJECTSCORE");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("UNLOCK", false),
						new KeyWord("OPEN", false)),
                    new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                        (actor, matchable) =>
                        {
                            if (matchable is ILockableRules && (matchable as ILockableRules).Locked)
                                return 1;
                            return -1;
                        }, "SUBJECTSCORE"), 
                    new KeyWord("WITH", true),
					new ObjectMatcher("OBJECT", new InScopeObjectSource(), 
                        ObjectMatcher.PreferHeld, "OBJECTSCORE")),
				new UnlockProcessor(),
				"Unlock something with something",
                "SUBJECTSCORE",
                "OBJECTSCORE");
		}
	}
	
	internal class UnlockProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as ILockableRules;
			var key = Match.Arguments["OBJECT"] as Thing;
			
			if (target == null)
			{
				if (Actor.ConnectedClient != null) 
					Mud.SendMessage(Actor, "I don't think the concept of 'locked' applies to that.\r\n");
				return;
			}

			if (!Object.ReferenceEquals(Actor, key.Location))
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "You'd have to be holding " + key.Definite + " for that to work.\r\n");
				return;
			}

			if (!target.Locked)
			{
				Mud.SendMessage(Actor, MessageScope.Single, "It's not locked.\r\n");
				return;
			}

			if (target.CanUnlock(Actor, key))
			{
				var thing = target as Thing;
				if (thing != null)
				{
					Mud.SendMessage(Actor, MessageScope.Single, "You unlock " + thing.Definite + ".\r\n");
					Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " unlocks " + thing.Indefinite + " with " + key.Indefinite + ".\r\n");
				}
				target.HandleUnlock(Actor, key);
			}
				else
			{
				Mud.SendMessage(Actor, MessageScope.Single, "It doesn't work.\r\n");
			}
		}
	}

	internal class LockProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as ILockableRules;
			var key = Match.Arguments["OBJECT"] as Thing;

			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "I don't think the concept of 'locked' applies to that.\r\n");
				return;
			}

			if (!Object.ReferenceEquals(Actor, key.Location))
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "You'd have to be holding " + key.Definite + " for that to work.\r\n");
				return;
			}

			if (target.Locked)
			{
				Mud.SendMessage(Actor, MessageScope.Single, "It's already locked.\r\n");
				return;
			}

			if (target.CanLock(Actor, key))
			{
				var thing = target as Thing;
				if (thing != null)
				{
					Mud.SendMessage(Actor, MessageScope.Single, "You lock " + thing.Definite + ".\r\n");
					Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " locks " + thing.Indefinite + " with " + key.Indefinite + ".\r\n");
				}
				target.HandleLock(Actor, key);
			}
			else
			{
				Mud.SendMessage(Actor, MessageScope.Single, "It doesn't work.\r\n");
			}
		}
	}

}
