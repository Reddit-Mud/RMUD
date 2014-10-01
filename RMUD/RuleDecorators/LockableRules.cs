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
		CheckRule CanLock(Actor Actor, Thing Key);
		CheckRule CanUnlock(Actor Actor, Thing Key);
		RuleHandlerFollowUp HandleLock(Actor Actor, Thing Key);
		RuleHandlerFollowUp HandleUnlock(Actor Actor, Thing Key);
	}
}
