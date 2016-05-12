using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

namespace RMUD
{
    public partial class RuleEngine : SharpRuleEngine.RuleEngine
    {
        public RuleEngine()
            : base(NewRuleQueueingMode.QueueNewRules)
        { }

        private IEnumerable<RuleSet> EnumerateMatchRules(PossibleMatch Match)
        {
            foreach (var arg in Match)
                if (arg.Value is MudObject && (arg.Value as MudObject).Rules != null)
                    yield return (arg.Value as MudObject).Rules;
        }

        /// <summary>
        /// Consider a perform rule, but check the values of the possible match for applicable rules. This can apply
        /// only to perform rules with the argument pattern [PossibleMatch, MudObject].
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Match"></param>
        /// <param name="Actor"></param>
        /// <returns></returns>
        public PerformResult ConsiderMatchBasedPerformRule(String Name, PossibleMatch Match, MudObject Actor)
        {
            return ConsiderPerformRule_Enum(Name, (args) => EnumerateMatchRules(Match), Match, Actor);
        }

        /// <summary>
        /// A wrapper around ConsiderCheckRule that maintains flags to suppress output from the mud.
        /// This is useful for when we want to check if an action would succeed (the rulebook returns 
        /// CheckResult.Allow) or fail (the rulebook returns CheckResult.Disallow) but we don't actually
        /// want to emit the failure messages these rules might normally generate.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        public CheckResult ConsiderCheckRuleSilently(String Name, params Object[] Arguments)
        {
            try
            {
                Core.SilentFlag = true;
                var r = ConsiderCheckRule(Name, Arguments);
                return r;
            }
            finally
            {
                Core.SilentFlag = false;
            }
        }
    }

}
