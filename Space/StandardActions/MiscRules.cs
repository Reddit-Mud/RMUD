using System;
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
            GlobalRules.Perform<Actor>("player joined")
                .Last
                .Do((actor) =>
                {
                    Core.EnqueuActorCommand(actor, "look");
                    return PerformResult.Continue;
                })
                .Name("New players look rule.");
        }
    }
}
