using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum NewRuleQueueingMode
    {
        QueueNewRules,
        ImmediatelyAddNewRules 
    }

    public partial class RuleEngine
    {
        internal Actor LogTo = null;
        internal void LogRules(Actor To) { LogTo = To; }

        public RuleSet Rules;
        internal NewRuleQueueingMode QueueingMode = NewRuleQueueingMode.ImmediatelyAddNewRules;
        internal List<Action> NewRuleQueue = new List<Action>();

        public RuleEngine(NewRuleQueueingMode QueueingMode)
        {
            this.QueueingMode = QueueingMode;
            Rules = new RuleSet(this);
        }

        /// <summary>
        /// After all initial rule setup has completed, call this to empty the new rule queue and process all 
        /// the added rules.
        /// </summary>
        public void FinalizeNewRules()
        {
            if (QueueingMode != NewRuleQueueingMode.QueueNewRules) return;
            QueueingMode = NewRuleQueueingMode.ImmediatelyAddNewRules;
            foreach (var act in NewRuleQueue) act();
            NewRuleQueue.Clear();
        }

        private void CreateNewRule(Action act)
        {
            if (QueueingMode == NewRuleQueueingMode.QueueNewRules) NewRuleQueue.Add(act);
            else act();
        }

        /// <summary>
        /// Do the types supplied match the types defined on an already declared global rulebook?
        /// Derived types 'match' their ancestor for the purposes of this function.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ResultType"></param>
        /// <param name="ArgumentCount"></param>
        /// <returns></returns>
        internal bool TypesAgreeWithDeclaredGlobalRuleBook(String Name, Type ResultType, int ArgumentCount)
        {
            if (Rules == null) return true; // This means that rules were declared before global rulebooks were discovered. Queueing prevents this from happening.

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;

            if (book.ResultType != ResultType) return false;
            if (book.ArgumentCount != ArgumentCount) return false;

            return true;
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            Rules.DeleteRule(RuleBookName, RuleID);
        }

        /// <summary>
        /// Given an array of Object, enumerate any and all of them that happen to be RuleSources. If they are a 
        /// RuleSource, include their LinkedRuleSet, if any, in the enumeration as well.
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        private IEnumerable<RuleSet> EnumerateRuleSets(Object[] Arguments)
        {
            if (Arguments != null)
            {
                // Avoid enumerating rules from the same object twice.
                var objectsExamined = new List<RuleSource>();

                foreach (var arg in Arguments)
                    if (arg is RuleSource
                        && !objectsExamined.Contains(arg as RuleSource)
                        && (arg as RuleSource).Rules != null)
                    {
                        objectsExamined.Add(arg as RuleSource);
                        yield return (arg as RuleSource).Rules;
                    }

                // Loop again, so that all linked rules are enumerated after all parent rules.
                foreach (var arg in Arguments)
                    if (arg is RuleSource)
                        if ((arg as RuleSource).LinkedRuleSource != null)
                            if (!objectsExamined.Contains((arg as RuleSource).LinkedRuleSource))
                                if ((arg as RuleSource).LinkedRuleSource.Rules != null)
                                {
                                    objectsExamined.Add((arg as RuleSource).LinkedRuleSource);
                                    yield return (arg as RuleSource).LinkedRuleSource.Rules;
                                }
            }
        }

        /// <summary>
        /// Consider a perform rulebook. First, check every argument for matching rules. Finally, check global rules.
        /// A perform rulebook continues execution until a rule returns PerformResult.Stop.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Consider a check rulebook. First check each argument for applicable rules, then check for global rules.
        /// A check rulebook continues until a rule returns CheckResult.Allow or CheckResult.Disallow.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Consider a perform rule, but check the values of the possible match for applicable rules. This can apply
        /// only to perform rules with the argument pattern <PossibleMatch, Actor>.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Match"></param>
        /// <param name="Actor"></param>
        /// <returns></returns>
        public PerformResult ConsiderMatchBasedPerformRule(String Name, PossibleMatch Match, Actor Actor)
        {
            foreach (var arg in Match)
                if (arg.Value is MudObject && (arg.Value as MudObject).Rules != null)
                    if ((arg.Value as MudObject).Rules.ConsiderPerformRule(Name, Match, Actor) == PerformResult.Stop)
                        return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Match, Actor);
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
