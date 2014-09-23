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
					new ObjectMatcher("TARGET", new InScopeObjectSource(), 
                        (actor, matchable) => {
                            if (matchable is ILockableRules && !(matchable as ILockableRules).Locked)
                                return 1;
                            return -1;
                        }),
					new KeyWord("WITH", true),
					new ObjectMatcher("KEY", new InScopeObjectSource(), ObjectMatcher.PreferHeld)),
				new LockProcessor(),
				"Lock something with something");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("UNLOCK", false),
						new KeyWord("OPEN", false)),
                    new ObjectMatcher("TARGET", new InScopeObjectSource(),
                        (actor, matchable) =>
                        {
                            if (matchable is ILockableRules && (matchable as ILockableRules).Locked)
                                return 1;
                            return -1;
                        }), 
                    new KeyWord("WITH", true),
					new ObjectMatcher("KEY", new InScopeObjectSource(), ObjectMatcher.PreferHeld)),
				new UnlockProcessor(),
				"Unlock something with something");
		}
	}
	
	internal class UnlockProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as ILockableRules;
			var key = Match.Arguments["KEY"] as Thing;
			
			if (target == null)
			{
				if (Actor.ConnectedClient != null) 
					Actor.ConnectedClient.Send("I don't think the concept of 'locked' applies to that.\r\n");
				return;
			}

			if (!Object.ReferenceEquals(Actor, key.Location))
			{
				if (Actor.ConnectedClient != null)
					Actor.ConnectedClient.Send("You'd have to be holding " + key.Definite + " for that to work.\r\n");
				return;
			}

			if (!target.Locked)
			{
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "It's not locked.\r\n");
				return;
			}

			if (target.CanUnlock(Actor, key))
			{
				var thing = target as Thing;
				if (thing != null)
				{
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You unlock " + thing.Definite + ".\r\n");
					Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " unlocks " + thing.Indefinite + " with " + key.Indefinite + ".\r\n");
				}
				target.HandleUnlock(Actor, key);
			}
				else
			{
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "It doesn't work.\r\n");
			}
		}
	}

	internal class LockProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as ILockableRules;
			var key = Match.Arguments["KEY"] as Thing;

			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Actor.ConnectedClient.Send("I don't think the concept of 'locked' applies to that.\r\n");
				return;
			}

			if (!Object.ReferenceEquals(Actor, key.Location))
			{
				if (Actor.ConnectedClient != null)
					Actor.ConnectedClient.Send("You'd have to be holding " + key.Definite + " for that to work.\r\n");
				return;
			}

			if (target.Locked)
			{
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "It's already locked.\r\n");
				return;
			}

			if (target.CanLock(Actor, key))
			{
				var thing = target as Thing;
				if (thing != null)
				{
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You lock " + thing.Definite + ".\r\n");
					Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " locks " + thing.Indefinite + " with " + key.Indefinite + ".\r\n");
				}
				target.HandleLock(Actor, key);
			}
			else
			{
				Mud.SendEventMessage(Actor, EventMessageScope.Single, "It doesn't work.\r\n");
			}
		}
	}

}
