using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class LockedDoor : BasicDoor, OpenableRules
	{
        public Func<MudObject, bool> IsMatchingKey;

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
        
		public bool Locked { get; set; }

	}
}
