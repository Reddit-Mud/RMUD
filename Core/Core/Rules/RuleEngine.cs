using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class RuleEngine
    {
        public RuleSet Rules;
        internal Actor LogTo = null;
        internal bool QueueNewRules = true;
        internal List<Action> NewRuleQueue = new List<Action>();

        public RuleEngine()
        {
            Rules = new RuleSet(this);
        }

        public void FinalizeNewRules()
        {
            QueueNewRules = false;
            foreach (var act in NewRuleQueue) act();
            NewRuleQueue.Clear();
        }

        private void NewRule(Action act)
        {
            if (QueueNewRules) NewRuleQueue.Add(act);
            else act();
        }

        internal void LogRules(Actor To) { LogTo = To; }

        internal bool CheckGlobalRuleBookTypes(String Name, Type ResultType, params Type[] ArgumentTypes)
        {
            if (Rules == null) return true; // This means that rules were declared before global rulebooks were discovered. The only object that does this in normal running is the settings object. So the settings object can potentially blow up everything.

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;

            if (book.ResultType != ResultType) return false;
            return book.CheckArgumentTypes(ResultType, ArgumentTypes);
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            Rules.DeleteRule(RuleBookName, RuleID);
        }

        public IEnumerable<RuleSet> EnumerateRuleSets(Object[] Arguments)
        {
            if (Arguments != null)
            {
                var objectsExamined = new List<MudObject>();

                foreach (var arg in Arguments)
                    if (arg is MudObject
                        && !objectsExamined.Contains(arg as MudObject)
                        && (arg as MudObject).Rules != null)
                    {
                        objectsExamined.Add(arg as MudObject);
                        yield return (arg as MudObject).Rules;
                    }

                foreach (var arg in Arguments)
                    if (arg is MudObject)
                        if ((arg as MudObject).Location != null)
                            if (!objectsExamined.Contains((arg as MudObject).Location))
                                if ((arg as MudObject).Location.Rules != null)
                                {
                                    objectsExamined.Add((arg as MudObject).Location);
                                    yield return (arg as MudObject).Location.Rules;
                                }
            }
        }

        public PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            //A single null value passed to a params argument is interpretted by C# as a null Object[]
            //reference, not as an array with a single element that is null.
            if (Arguments == null) Arguments = new Object[] { null };

            foreach (var ruleset in EnumerateRuleSets(Arguments))
                if (ruleset.ConsiderPerformRule(Name, Arguments) == PerformResult.Stop)
                    return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Arguments);
        }

        public PerformResult ConsiderMatchBasedPerformRule(String Name, PossibleMatch Match, Actor Actor)
        {
            foreach (var arg in Match)
                if (arg.Value is MudObject && (arg.Value as MudObject).Rules != null)
                    if ((arg.Value as MudObject).Rules.ConsiderPerformRule(Name, Match, Actor) == PerformResult.Stop)
                        return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Match, Actor);
        }

        public CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            if (Arguments == null) Arguments = new Object[] { null };

            foreach (var ruleset in EnumerateRuleSets(Arguments))
            {
                var r = ruleset.ConsiderCheckRule(Name, Arguments);
                if (r != CheckResult.Continue) return r;
            }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderCheckRule(Name, Arguments);
        }

        public RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            if (Arguments == null) Arguments = new Object[] { null };
            
            bool valueReturned = false;

            foreach (var ruleset in EnumerateRuleSets(Arguments))
            {
                var r = ruleset.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
                if (valueReturned) return r;
            }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
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
