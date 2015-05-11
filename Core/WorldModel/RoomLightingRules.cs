using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class RoomLightingRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("light level", "[item] -> LightingLevel, How much light does the item emit?", "item");

            GlobalRules.Value<MudObject, LightingLevel>("light level")
                .Do(item => LightingLevel.Dark)
                .Name("Items emit no light by default rule.");

            GlobalRules.Perform<Room>("update")
                .Do(room =>
                {
                    room.UpdateLighting();
                    return PerformResult.Continue;
                })
                .Name("Update room lighting rule.");
        }

        public static RuleBuilder<MudObject, LightingLevel> ValueLightingLevel(this MudObject Object)
        {
            return Object.Value<MudObject, LightingLevel>("light level").ThisOnly();
        }

    }
}
