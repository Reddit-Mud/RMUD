using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class SillyStatusEffect : StatusEffect
    {
        int Counter;

        public override void Apply(Actor To)
        {
            To.Nouns.Add("silly");
            Counter = 100;
            Mud.RegisterForHeartbeat(To);

            To.AddValueRule<MudObject, MudObject, String, String>("actor-name").Do((viewer, actor, article) =>
                {
                    return "silly " + (actor as Actor).Short;
                }).Name("Silly name rule").ID("SILLYSTATUSEFFECT");
        }

        public override void Remove(Actor From)
        {
            From.Nouns.Remove("silly");
            From.Rules.DeleteRule("actor-name", "SILLYSTATUSEFFECT");
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
