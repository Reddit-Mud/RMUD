using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace StandardActionsModule
{
    public static class MiscCustomization
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {

        }

        public static CheckResult CheckIsVisibleTo(MudObject Actor, MudObject Item)
        {
            if (!MudObject.IsVisibleTo(Actor, Item))
            {
                MudObject.SendMessage(Actor, "That doesn't seem to be here any more.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }

        public static CheckResult CheckIsHolding(MudObject Actor, MudObject Item)
        {
            if (!MudObject.ObjectContainsObject(Actor, Item))
            {
                MudObject.SendMessage(Actor, "You don't have that.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }
    }
}
