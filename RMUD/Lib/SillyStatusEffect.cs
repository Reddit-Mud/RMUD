using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class SillyStatusEffect : StatusEffect
    {
        String SaveShort;
        int Counter;

        public override void Apply(Actor To)
        {
            SaveShort = To.Short;
            To.Short = "silly " + SaveShort;
            To.Nouns.Add("silly");

            Counter = 100;
            Mud.RegisterForHeartbeat(To);
        }

        public override void Remove(Actor From)
        {
            From.Short = SaveShort;
            From.Nouns.Remove("silly");
        }

        public override void Heartbeat(ulong HeartbeatID, Actor AppliedTo)
        {
            Counter -= 1;
            if (Counter <= 0)
            {
                Mud.SendExternalMessage(AppliedTo, "^<the0> is serious now.", AppliedTo);
                AppliedTo.RemoveStatusEffect(this);
            }
        }
    }
}
