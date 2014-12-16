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

            To.Value<MudObject, MudObject, String, String>("printed-name")
                .Do((viewer, actor, article) =>
                {
                    return "silly " + (actor as Actor).Short;
                })
                .Name("Silly name rule")
                .ID("SILLYSTATUSEFFECT");

            GlobalRules.Perform("heartbeat")
                .Do(() =>
                {
                    Counter -= 1;
                    if (Counter <= 0)
                    {
                        Mud.SendExternalMessage(To, "^<the0> is serious now.", To);
                        To.Nouns.Remove("silly");
                        To.Rules.DeleteAll("SILLYSTATUSEFFECT");
                        GlobalRules.DeleteRule("heartbeat", "SILLYSTATUSEFFECT");
                    }
                    return PerformResult.Continue;
                })
                .ID("SILLYSTATUSEFFECT");
        }

    }
}
