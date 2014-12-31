using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class HeartbeatRules : DeclaresRules
    {
        public void InitializeRules()
        {
            GlobalRules.DeclarePerformRuleBook("heartbeat", "[] : Considered every tick.");
        }
    }

    public static partial class Core
    {
        internal static DateTime TimeOfLastHeartbeat = DateTime.Now;
		
        internal static void Heartbeat()
        {
            var now = DateTime.Now;
            var timeSinceLastBeat = now - TimeOfLastHeartbeat;
            if (timeSinceLastBeat.TotalMilliseconds >= SettingsObject.HeartbeatInterval)
            {
                MudObject.TimeOfDay += SettingsObject.ClockAdvanceRate;

                TimeOfLastHeartbeat = now;
                GlobalRules.ConsiderPerformRule("heartbeat");

                MudObject.SendPendingMessages();
            }
        }
    }
}
