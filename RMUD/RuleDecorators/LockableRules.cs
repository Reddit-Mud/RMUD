using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	/// <summary>
	/// Decorates an object that can be locked or unlocked, and interactive with the lock and unlock commands.
	/// </summary>
	public interface LockableRules
	{
		bool Locked { get; }
		CheckRule CheckLock(Actor Actor, MudObject Key);
		CheckRule CheckUnlock(Actor Actor, MudObject Key);
		RuleHandlerFollowUp HandleLock(Actor Actor, MudObject Key);
		RuleHandlerFollowUp HandleUnlock(Actor Actor, MudObject Key);
	}
}
