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

            AddCheckRule<MudObject, MudObject, MudObject>("can-be-locked-with").Do((actor, door, key) =>
                {
                    if (Open) {
                        Mud.SendMessage(actor, "You'll have to close it first.");
                        return CheckResult.Disallow;
                    }

                    if (!IsMatchingKey(key))
                    {
                        Mud.SendMessage(actor, "That is not the right key.");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Allow;
                });

            AddPerformRule<MudObject, MudObject, MudObject>("on-locked-with").Do((a,b,c) =>
                {
                    Locked = true;
                    return PerformResult.Continue;
                });

             AddPerformRule<MudObject, MudObject, MudObject>("on-unlocked-with").Do((a,b,c) =>
                {
                    Locked = false;
                    return PerformResult.Continue;
                });

             AddCheckRule<MudObject, MudObject>("can-open").When((a, b) => Locked).Do((a, b) =>
                 {
                     Mud.SendMessage(a, "It seems to be locked.");
                     return CheckResult.Disallow;
                 });

             AddPerformRule<MudObject, MudObject>("on-closed").Do((a, b) => { Locked = false; return PerformResult.Continue; });
        }
        
	}
}
