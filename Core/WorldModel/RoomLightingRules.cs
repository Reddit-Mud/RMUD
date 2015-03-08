using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RoomLightingRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("emits-light", "[item] -> LightingLevel, How much light does the item emit?", "item");
            GlobalRules.Value<MudObject, LightingLevel>("emits-light").Do(item => LightingLevel.Dark);

            GlobalRules.Perform<MudObject>("update")
                .When(thing => thing is Room)
                .Do(thing =>
                {
                    (thing as Room).UpdateLighting();
                    return PerformResult.Continue;
                })
                .Name("Update room lighting rule.");
        }
    }
}
