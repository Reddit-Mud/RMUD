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
        public void InitializeGlobalRules()
        {
            GlobalRules.DeclarePerformRuleBook("heartbeat", "[] : Considered every tick.");
        }
    }

    public static partial class Mud
    {
        internal static DateTime TimeOfLastHeartbeat = DateTime.Now;
        internal static DateTime TimeOfDay = DateTime.Parse("03/15/2015 11:15:00 -5:00");
		
        internal static void Heartbeat()
        {
            var now = DateTime.Now;
            var timeSinceLastBeat = now - TimeOfLastHeartbeat;
            if (timeSinceLastBeat.TotalMilliseconds >= Mud.SettingsObject.HeartbeatInterval)
            {
                TimeOfDay += Mud.SettingsObject.ClockAdvanceRate;

                TimeOfLastHeartbeat = now;
                GlobalRules.ConsiderPerformRule("heartbeat", null);

                Mud.SendPendingMessages();
            }
        }
    }
}
