using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class LockedDoor : Thing, IOpenableRules, ITakeRules, ILockableRules
	{
        public Func<Thing, bool> IsMatchingKey;

		public LockedDoor()
		{
			this.Nouns.Add("DOOR", "CLOSED");
			Open = false;
			Locked = true;
		}

		#region IOpenable

		public bool Open { get; set; }

		CheckRule IOpenableRules.CanOpen(Actor Actor)
		{
            if (Locked) return CheckRule.Disallow("It seems to be locked.");
            if (Open) return CheckRule.Disallow("It's already open.");
            else return CheckRule.Allow();
		}

		CheckRule IOpenableRules.CanClose(Actor Actor)
		{
            if (!Open) return CheckRule.Disallow("It's already closed.");
            else return CheckRule.Allow();
		}

		RuleHandlerFollowUp IOpenableRules.HandleOpen(Actor Actor)
		{
			Open = true;
            Nouns.RemoveAll(n => n == "CLOSED");
            Nouns.Add("OPEN");
            return RuleHandlerFollowUp.Continue;
		}

		RuleHandlerFollowUp IOpenableRules.HandleClose(Actor Actor)
		{
			Open = false;
			Locked = false;
            Nouns.RemoveAll(n => n == "OPEN");
            Nouns.Add("CLOSED");
            return RuleHandlerFollowUp.Continue;
		}

		#endregion

		CheckRule ITakeRules.CanTake(Actor Actor)
		{
			return CheckRule.Disallow("That's not going to work.");
		}

        RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor) { return RuleHandlerFollowUp.Continue; }

		#region ILockableRules

		public bool Locked { get; set; }

		CheckRule ILockableRules.CanLock(Actor Actor, Thing Key)
		{
			if (Open) return CheckRule.Disallow("You'll have to close it first.");
            if (Locked) return CheckRule.Disallow("It's already locked.");
            if (IsMatchingKey(Key))
                return CheckRule.Allow();
            else
                return CheckRule.Disallow("That is not the right key.");
		}

		CheckRule ILockableRules.CanUnlock(Actor Actor, Thing Key)
		{
            if (Open) return CheckRule.Disallow("It's already open.");
            if (!Locked) return CheckRule.Disallow("It's not locked.");
            if (IsMatchingKey(Key))
                return CheckRule.Allow();
            else
                return CheckRule.Disallow("That is not the right key.");
		}

		RuleHandlerFollowUp ILockableRules.HandleLock(Actor Actor, Thing Key)
		{
			Locked = true;
            return RuleHandlerFollowUp.Continue;
		}

		RuleHandlerFollowUp ILockableRules.HandleUnlock(Actor Actor, Thing Key)
		{
			Locked = false;
            return RuleHandlerFollowUp.Continue;
		}

		#endregion
	}
}
