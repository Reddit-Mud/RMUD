﻿using RMUD;

namespace CloakOfDarkness
{
    public class Cloak : RMUD.MudObject
    {
        bool isHung = false;

        public static void AtStartup(RuleEngine GlobalRules)
        {

        }

        public override void Initialize()
        {
            /*
 The player wears a velvet cloak. The cloak can be hung or unhung.
Understand "dark" or "black" or "satin" as the cloak.
The description of the cloak is "A handsome cloak,
of velvet trimmed with satin, and slightly splattered with raindrops.
Its blackness is so deep that it almost seems to suck light from the room."
*/
            SetProperty("clothing layer", ClothingModule.ClothingLayer.Outer);
            SetProperty("clothing part", ClothingModule.ClothingBodyPart.Cloak);
            SetProperty("wearable?", true);

            SetProperty("short", "velvet cloak");
            GetProperty<NounList>("nouns").Add("dark", "black", "satin", "velvet", "cloak");
            SetProperty("long", "A handsome cloak, of velvet trimmed with satin, and slightly spattered with raindrops. Its blackness is so deep that it almost seems to suck light from the room.");

            //Carry out taking the cloak:
            //    now the bar is dark.
            Perform<MudObject, MudObject>("take").Do((actor, thing) => { GetObject("Bar").SetProperty("ambient light", LightingLevel.Dark); return SharpRuleEngine.PerformResult.Continue; });

            //Carry out putting the unhung cloak on something in the cloakroom:
            //    now the cloak is hung;
            //    increase score by 1.
            //Perform<MudObject, MudObject, MudObject, RelativeLocations>("put")
            //    .When((actor, item, container, relloc) => container is Hook)
            //    .Do((actor, item, container, relloc) => { isHung = true; return PerformResult.Continue; });

            //Carry out putting the cloak on something in the cloakroom:
            //    now the bar is lit.
            Perform<MudObject, MudObject, MudObject, RelativeLocations>("put")
                .When((actor, item, container, relloc) => actor.Location is Cloakroom)
                .Do((actor, item, container, relloc) => { GetObject("Bar").SetProperty("ambient light", LightingLevel.Bright); return SharpRuleEngine.PerformResult.Continue; });

            //Carry out dropping the cloak in the cloakroom:
            //    now the bar is lit.
            Perform<MudObject, MudObject>("drop")
                .When((actor, item) => actor.Location is Cloakroom)
                .Do((actor, item) => { GetObject("Bar").SetProperty("ambient light", LightingLevel.Bright); return SharpRuleEngine.PerformResult.Continue; });

            //Instead of dropping or putting the cloak on when the player is not in the cloakroom:
            //  say "This isn't the best place to leave a smart cloak lying around."
            // * Since putting checks dropping, we only need one rule.
            Check<MudObject, MudObject>("can drop?")
                .When((actor, item) => !(actor.Location is Cloakroom))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "This isn't the best place to leave a smart cloak lying around.");
                    return SharpRuleEngine.CheckResult.Disallow;
                });
        }
    }
}
