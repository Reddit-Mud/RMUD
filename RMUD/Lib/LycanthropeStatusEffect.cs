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

            To.Value<MudObject, MudObject, String, String>("actor-name")
                .Do((viewer, actor, article) => article + " beast")
                .Name("Lycanthrope actor name")
                .ID("LYCANTHROPE");
        }

        public override void Remove(Actor From)
        {
            From.Nouns.Remove("beast");
            From.Rules.DeleteAll("LYCANTHROPE");
        }

    }
}
