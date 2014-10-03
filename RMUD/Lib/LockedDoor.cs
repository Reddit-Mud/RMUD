using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class LockedDoor : BasicDoor, OpenableRules, LockableRules
	{
        public Func<MudObject, bool> IsMatchingKey;

		public LockedDoor()
		{
			Locked = true;
		}

		#region IOpenable

		CheckRule OpenableRules.CheckOpen(Actor Actor)
		{
            if (Locked) return CheckRule.Disallow("It seems to be locked.");
            if (Open) return CheckRule.Disallow("It's already open.");
            else return CheckRule.Allow();
		}

		CheckRule OpenableRules.CheckClose(Actor Actor)
		{
            if (!Open) return CheckRule.Disallow("It's already closed.");
            else return CheckRule.Allow();
		}

		RuleHandlerFollowUp OpenableRules.HandleOpen(Actor Actor)
		{
            return ImplementHandleOpen(Actor);
		}

        RuleHandlerFollowUp OpenableRules.HandleClose(Actor Actor)
        {
            Locked = false;
            return ImplementHandleClose(Actor);
        }

		#endregion
        
		#region ILockableRules

		public bool Locked { get; set; }

		CheckRule LockableRules.CheckLock(Actor Actor, MudObject Key)
		{
			if (Open) return CheckRule.Disallow("You'll have to close it first.");
            if (Locked) return CheckRule.Disallow("It's already locked.");
            if (IsMatchingKey(Key))
                return CheckRule.Allow();
            else
                return CheckRule.Disallow("That is not the right key.");
		}

		CheckRule LockableRules.CheckUnlock(Actor Actor, MudObject Key)
		{
            if (Open) return CheckRule.Disallow("It's already open.");
            if (!Locked) return CheckRule.Disallow("It's not locked.");
            if (IsMatchingKey(Key))
                return CheckRule.Allow();
            else
                return CheckRule.Disallow("That is not the right key.");
		}

		RuleHandlerFollowUp LockableRules.HandleLock(Actor Actor, MudObject Key)
		{
			Locked = true;
            return RuleHandlerFollowUp.Continue;
		}

		RuleHandlerFollowUp LockableRules.HandleUnlock(Actor Actor, MudObject Key)
		{
			Locked = false;
            return RuleHandlerFollowUp.Continue;
		}

		#endregion
	}
}
