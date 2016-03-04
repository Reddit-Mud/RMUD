using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace DelmudGameplay
{
    public class Monster : MudObject
    {
        public override void Initialize()
        {
            Actor();

            Perform<MudObject, MudObject>("killed")
                .Do((me, attacker) =>
                {
                    SendLocaleMessage(me, "^<a0> killed <a1>!", attacker, me);

                    Move(me, null);

                    return SharpRuleEngine.PerformResult.Continue;
                });

            
        }
    }
}
