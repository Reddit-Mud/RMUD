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

		bool IOpenableRules.CanOpen(Actor Actor)
		{
			if (Locked) return false;
			return !Open;
		}

		bool IOpenableRules.CanClose(Actor Actor)
		{
			return Open;
		}

		void IOpenableRules.HandleOpen(Actor Actor)
		{
			Open = true;
            Nouns.RemoveAll(n => n == "CLOSED");
            Nouns.Add("OPEN");
		}

		void IOpenableRules.HandleClose(Actor Actor)
		{
			Open = false;
			Locked = false;
            Nouns.RemoveAll(n => n == "OPEN");
            Nouns.Add("CLOSED");
		}

		#endregion

		bool ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}

		void ITakeRules.HandleTake(Actor Actor) { }

		#region ILockableRules

		public bool Locked { get; set; }

		bool ILockableRules.CanLock(Actor Actor, Thing Key)
		{
			if (Open) return false;
			if (Locked) return false;
            return IsMatchingKey(Key);
		}

		bool ILockableRules.CanUnlock(Actor Actor, Thing Key)
		{
			if (Open) return false;
			if (!Locked) return false;
            return IsMatchingKey(Key);
		}

		void ILockableRules.HandleLock(Actor Actor, Thing Key)
		{
			Locked = true;
		}

		void ILockableRules.HandleUnlock(Actor Actor, Thing Key)
		{
			Locked = false;
		}

		#endregion
	}
}
