using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

namespace RMUD
{
    /// <summary>
    /// This is a fancy door - it can be locked!
    /// 
    /// TODO: "IsMatchingKey" should be replaced with a rule.
    /// TODO: "Locked" should be a property.
    /// TODO: Sync locked state with opposite side of portal
    /// </summary>
	public class LockedDoor : BasicDoor
	{
        public Func<MudObject, bool> IsMatchingKey;

        public bool Locked { get; set; }

		public LockedDoor()
		{
			Locked = true;

            SetProperty("lockable?", true);

            Check<MudObject, MudObject, MudObject>("can lock?").Do((actor, door, key) =>
                {
                    if (GetPropertyOrDefault<bool>("open?")) {
                        MudObject.SendMessage(actor, "@close it first");
                        return CheckResult.Disallow;
                    }

                    if (!IsMatchingKey(key))
                    {
                        MudObject.SendMessage(actor, "@wrong key");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Allow;
                });

            Perform<MudObject, MudObject, MudObject>("locked").Do((a,b,c) =>
                {
                    Locked = true;
                    return PerformResult.Continue;
                });

             Perform<MudObject, MudObject, MudObject>("unlocked").Do((a,b,c) =>
                {
                    Locked = false;
                    return PerformResult.Continue;
                });

             Check<MudObject, MudObject>("can open?")
                 .First
                 .When((a, b) => Locked)
                 .Do((a, b) =>
                 {
                     MudObject.SendMessage(a, "@error locked");
                     return CheckResult.Disallow;
                 })
                 .Name("Can't open locked door rule.");

             Perform<MudObject, MudObject>("close")
                 .Do((a, b) => { Locked = false; return PerformResult.Continue; });
        }
        
	}
}
