using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RoomLightingRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("emits-light", "[item] -> LightingLevel, How much light does the item emit?");
            GlobalRules.Value<MudObject, LightingLevel>("emits-light").Do(item => LightingLevel.Dark);
        }
    }
}
