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

            Perform<MudObject, MudObject>("attack")
                .When((actor, victim) => Object.ReferenceEquals(victim, this))
                .When((actor, victim) => actor.State == ObjectState.Alive && victim.State == ObjectState.Alive)
                .Do((actor, victim) =>
                {
                    // Now actor is an aggressor.
                    victim.SetProperty("combatant", actor);
                    Core.AddTimer(TimeSpan.FromSeconds(2), () => ConsiderPerformRule("attack", victim, actor));
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Fight back rule");

            Perform<MudObject, MudObject>("attack")
                .When((actor, victim) => Object.ReferenceEquals(actor, this))
                .When((actor, victim) => actor.State == ObjectState.Alive && victim.State == ObjectState.Alive)
                .Do((actor, victim) =>
                {
                    actor.SetProperty("combatant", victim);
                    Core.AddTimer(TimeSpan.FromSeconds(2), () => ConsiderPerformRule("attack", actor, victim));
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Keep attacking rule.");
        }
    }
}
