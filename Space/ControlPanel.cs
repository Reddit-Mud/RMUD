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
        }

        public ControlPanel()
        {
            SimpleName("control panel", "controls");
        }
    }
}
