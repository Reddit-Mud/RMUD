using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using SharpRuleEngine;

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
                    return SharpRuleEngine.PerformResult.Continue;
                }).Name("Advance clock on heartbeat rule");
        }
    }

    internal class Timer
    {
        public DateTime StartTime;
        public TimeSpan Interval;
        public MudObject InvokeOn;
        public String Rule;
        public Object[] RuleArguments;
    }

    public static partial class Core
    {
        public static PerformResult ConsiderLocalOnlyPerformRule(MudObject On, String Name, Object[] Arguments)
        {
            if (Arguments == null) Arguments = new Object[] { null };
            var ruleset = On.Rules;
            return ruleset.ConsiderPerformRule(Name, Arguments);
        }

        internal static List<Timer> ActiveTimers = new List<Timer>();

        public static void AddTimer(TimeSpan Interval, MudObject On, String Rule, params Object[] Arguments)
        {
            if (Interval < TimeSpan.FromSeconds(1))
            {
                Interval = TimeSpan.FromSeconds(1);
                Core.LogWarning("Timer created with interval less than minimum value.");
            }

            ActiveTimers.Add(new Timer
            {
                StartTime = DateTime.Now,
                Interval = Interval,
                InvokeOn = On,
                Rule = Rule,
                RuleArguments = Arguments
            });
        }

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

            for (var i = 0; i < ActiveTimers.Count;)
            {
                var timerFireTime = ActiveTimers[i].StartTime + ActiveTimers[i].Interval;
                if (timerFireTime <= now)
                {
                    ConsiderLocalOnlyPerformRule(ActiveTimers[i].InvokeOn, ActiveTimers[i].Rule, ActiveTimers[i].RuleArguments);
                    Core.SendPendingMessages();
                    ActiveTimers.RemoveAt(i);
                }
                else
                    ++i;
            }
        }
    }
}
