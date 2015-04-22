using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class HeartbeatRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook("heartbeat", "[] : Considered every tick.");

            GlobalRules.Perform("heartbeat").Do(() =>
                {
                    MudObject.TimeOfDay += Core.SettingsObject.ClockAdvanceRate;
                    return PerformResult.Continue;
                }).Name("Advance clock on heartbeat rule");
        }
    }

    public static partial class Core
    {
        internal static DateTime TimeOfLastHeartbeat = DateTime.Now;
		
        /// <summary>
        /// Process the Heartbeat. It is assumed that this function is called periodically by the command processing loop.
        /// When called, this function will invoke the "heartbeat" rulebook if enough time has passed since the last
        /// invokation. 
        /// </summary>
        internal static void Heartbeat()
        {
            var now = DateTime.Now;
            var timeSinceLastBeat = now - TimeOfLastHeartbeat;
            if (timeSinceLastBeat.TotalMilliseconds >= SettingsObject.HeartbeatInterval)
            {
                TimeOfLastHeartbeat = now;
                GlobalRules.ConsiderPerformRule("heartbeat");

                //In case heartbeat rules emitted messages.
                Core.SendPendingMessages();
            }
        }
    }
}
