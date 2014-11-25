using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class StatusEffectGate : CommandTokenMatcher
    {
        public Type StatusEffectType;

        public StatusEffectGate(Type StatusEffectType)
        {
            this.StatusEffectType = StatusEffectType;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
            if (Context.ExecutingActor.HasStatusEffect(StatusEffectType))
                R.Add(State);
            return R;
        }

		public String Emit() { return "<Must have status effect " + StatusEffectType.Name + ">"; }
    }
}
