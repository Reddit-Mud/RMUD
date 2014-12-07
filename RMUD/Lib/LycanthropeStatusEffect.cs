using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class LycanthropeStatusEffect : StatusEffect
    {
        public override void Apply(Actor To)
        {
            To.Nouns.Add("beast", "wolf", "were", "werewolf");
        }

        public override void Remove(Actor From)
        {
            From.Nouns.Remove("beast");
        }

        public override Tuple<bool,String> OverrideName(Actor For, String PreviousName)
        {
            return new Tuple<bool, string>(true, "beast");
        }
    }
}
