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

            AddActionRule<MudObject, MudObject, MudObject>("can-be-locked-with").Do((actor, door, key) =>
                {
                    if (Open) {
                        Mud.SendMessage(actor, "You'll have to close it first.");
                        return RuleResult.Disallow;
                    }

                    if (!IsMatchingKey(key))
                    {
                        Mud.SendMessage(actor, "That is not the right key.");
                        return RuleResult.Disallow;
                    }

                    return RuleResult.Allow;
                });

            AddActionRule<MudObject, MudObject, MudObject>("on-locked-with").Do((a,b,c) =>
                {
                    Locked = true;
                    return RuleResult.Continue;
                });

             AddActionRule<MudObject, MudObject, MudObject>("on-unlocked-with").Do((a,b,c) =>
                {
                    Locked = false;
                    return RuleResult.Continue;
                });

             AddActionRule<MudObject, MudObject>("can-open").When((a, b) => Locked).Do((a, b) =>
                 {
                     Mud.SendMessage(a, "It seems to be locked.");
                     return RuleResult.Disallow;
                 });

             AddActionRule<MudObject, MudObject>("on-closed").Do((a, b) => { Locked = false; return RuleResult.Continue; });
        }
        
	}
}
