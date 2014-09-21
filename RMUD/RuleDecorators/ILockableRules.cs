using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	/// <summary>
	/// Decorates an object that can be locked or unlocked, and interactive with the lock and unlock commands.
	/// </summary>
	public interface ILockableRules
	{
		bool Locked { get; }
		bool CanLock(Actor Actor, Thing Key);
		bool CanUnlock(Actor Actor, Thing Key);
		void HandleLock(Actor Actor, Thing Key);
		void HandleUnlock(Actor Actor, Thing Key);
	}
}
