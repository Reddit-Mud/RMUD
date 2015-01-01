using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class LockedDoor : BasicDoor
	{
        public Func<MudObject, bool> IsMatchingKey;

        public bool Locked { get; set; }

		public LockedDoor()
		{
			Locked = true;

            Value<MudObject, bool>("lockable?").Do(a => true);

            Check<MudObject, MudObject, MudObject>("can lock?").Do((actor, door, key) =>
                {
                    if (Open) {
                        MudObject.SendMessage(actor, "You'll have to close it first.");
                        return CheckResult.Disallow;
                    }

                    if (!IsMatchingKey(key))
                    {
                        MudObject.SendMessage(actor, "That is not the right key.");
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
                     MudObject.SendMessage(a, "It seems to be locked.");
                     return CheckResult.Disallow;
                 })
                 .Name("Can't open locked door rule.");

             Perform<MudObject, MudObject>("closed")
                 .Do((a, b) => { Locked = false; return PerformResult.Continue; });
        }
        
	}
}
