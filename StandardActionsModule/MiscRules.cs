﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace StandardActionsModule
{
    public static class MiscRules
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            // This rule is never called in a single player context.
            GlobalRules.Perform<MudObject>("player joined")
                .Last
                .Do((actor) =>
                {
                    Core.EnqueuActorCommand(actor, "look");
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("New players look rule.");
        }
    }
}
