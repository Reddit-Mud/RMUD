using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

namespace RMUD
{
    public class RuleEngine : SharpRuleEngine.RuleEngine
    {
        public RuleEngine()
            : base(NewRuleQueueingMode.QueueNewRules)
        { }

        internal Actor LogTo = null;
        internal void LogRules(Actor To) { LogTo = To; }

        public PerformResult ConsiderMatchBasedPerformRule(String Name, PossibleMatch Match, Actor Actor)
        {
            foreach (var arg in Match)
                if (arg.Value is MudObject && (arg.Value as MudObject).Rules != null)
                    if ((arg.Value as MudObject).Rules.ConsiderPerformRule(Name, Match, Actor) == PerformResult.Stop)
                        return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Match, Actor);
        }

        public CheckResult ConsiderCheckRuleSilently(String Name, params Object[] Arguments)
        {
            try
            {
                Core.SilentFlag = true;
                var r = ConsiderCheckRule(Name, Arguments);
                Core.SilentFlag = false;
                return r;
            }
            finally
            {
                Core.SilentFlag = false;
            }
        }
    }

    public partial class MudObject
    {
        public static RuleEngine GlobalRules { get { return Core.GlobalRules; } }

        public static PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderPerformRule(Name, Arguments);
        }

        public static CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderCheckRule(Name, Arguments);
        }

        public static RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderValueRule<RT>(Name, Arguments);
        }

        public static CheckResult ConsiderCheckRuleSilently(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderCheckRuleSilently(Name, Arguments);
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            if (Rules != null) Rules.DeleteRule(RuleBookName, RuleID);
        }

        public void DeleteAllRules(String RuleID)
        {
            if (Rules != null) Rules.DeleteAll(RuleID);
        }
    }
}
