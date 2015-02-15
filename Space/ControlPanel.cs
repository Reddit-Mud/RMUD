using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace Space
{
    public class ControlPanel : MudObject
    {
        public enum IndicatorState
        {
            green,
            red
        }

        public IndicatorState Indicator = IndicatorState.green;
        public bool Broken = false;

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.Check<Actor, ControlPanel>("can take?")
                .Do((actor, panel) =>
                {
                    SendMessage(actor, "I can't take it. It's firmly attached.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Perform<Actor, ControlPanel>("describe")
                .When((actor, panel) => !panel.Broken)
                .Do((actor, panel) =>
                {
                    if (panel.Location is Hatch)
                    {
                        var thisSide = FindLocale(panel) as Room;
                        var otherSide = Portal.FindOppositeSide(panel.Location).Location as Room;
                        if (thisSide != null && otherSide != null && thisSide.AirLevel == otherSide.AirLevel)
                            panel.Indicator = IndicatorState.green;
                        else
                            panel.Indicator = IndicatorState.red;
                    }

                    SendMessage(actor, "It's a little square panel covered in buttons. There is a <s0> light on it.", panel.Indicator.ToString());
                    return PerformResult.Continue;
                });

            GlobalRules.Perform<Actor, ControlPanel>("describe")
                .When((actor, panel) => panel.Broken)
                .Do((actor, panel) =>
                {
                    SendMessage(actor, "It's been smashed up real good.");
                    return PerformResult.Continue;
                });

            GlobalRules.Perform<Player, Space.ControlPanel, MudObject>("hit with")
                .When((player, panel, wrench) => wrench.GetPropertyOrDefault<Weight>("weight", Weight.Normal) == Weight.Heavy)
                .Do((player, panel, wrench) =>
                {
                    SendMessage(player, "The panel smashed up good.");
                    panel.Broken = true;
                    return PerformResult.Stop;
                });
        }

        public ControlPanel()
        {
            SimpleName("control panel", "controls");
        }
    }
}
