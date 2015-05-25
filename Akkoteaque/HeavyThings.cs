using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace Akko
{
    public static class HeavyThings
    {
        public static bool IsHeavy(MudObject Object)
        {
            return Object.GetBooleanProperty("heavy?");
        }

        public static bool HasHeavyThing(MudObject Object)
        {
            var container = Object as Container;
            if (container == null) return false;
            return container.EnumerateObjects().Count(i => IsHeavy(i)) != 0;
        }

        public static MudObject FirstHeavyThing(MudObject Object)
        {
            var container = Object as Container;
            if (container == null) return null;
            return container.EnumerateObjects().FirstOrDefault(i => IsHeavy(i));
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            // Heavy things can be picked up, but not carried around.

            Core.StandardMessage("cant carry heavy thing", "^<the0> is too heavy to carry around.");

            GlobalRules.Check<MudObject, MudObject>("can go?")
                .When((actor, link) => HasHeavyThing(actor))
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "@cant carry heavy thing", FirstHeavyThing(actor));
                    return CheckResult.Disallow;
                })
                .Name("Can't carry around heavy things rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can push direction?")
                .When((actor, subject, link) => HasHeavyThing(actor))
                .Do((actor, subject, link) =>
                {
                    MudObject.SendMessage(actor, "@cant carry heavy thing", FirstHeavyThing(actor));
                    return CheckResult.Disallow;
                })
                .Name("Can't carry around heavy things while pushing rule.");
        }

        public static void Heavy(this MudObject Object)
        {
            Object.SetProperty("heavy", true);
        }
    }
}