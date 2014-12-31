using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface DeclaresRules
    {
        void InitializeRules();
    }

    public interface HasRules
    {
        RuleSet Rules { get; }
    }

    public static partial class GlobalRules
    {
        internal static RuleSet Rules = null;
        internal static Client LogTo = null;

        public static void LogRules(Client To) { LogTo = To; }

        public static bool CheckGlobalRuleBookTypes(String Name, Type ResultType, params Type[] ArgumentTypes)
        {
            if (Rules == null) return true; // This means that rules were declared before global rulebooks were discovered. The only object that does this in normal running is the settings object. So the settings object can potentially blow up everything.

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;

            if (book.ResultType != ResultType) return false;
            return book.CheckArgumentTypes(ResultType, ArgumentTypes);
        }

        public static void DeleteRule(String RuleBookName, String RuleID)
        {
            Rules.DeleteRule(RuleBookName, RuleID);
        }

        public static PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            foreach (var arg in Arguments)
                if (arg is HasRules && (arg as HasRules).Rules != null)
                    if ((arg as HasRules).Rules.ConsiderPerformRule(Name, Arguments) == PerformResult.Stop)
                        return PerformResult.Stop;

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderPerformRule(Name, Arguments);
        }

        public static CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            foreach (var arg in Arguments)
                if (arg is HasRules && (arg as HasRules).Rules != null)
                {
                    var r = (arg as HasRules).Rules.ConsiderCheckRule(Name, Arguments);
                    if (r != CheckResult.Continue) return r;
                }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderCheckRule(Name, Arguments);
        }

        public static RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            bool valueReturned = false;

            foreach (var arg in Arguments)
                if (arg is HasRules && (arg as HasRules).Rules != null)
                {
                    var r = (arg as HasRules).Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
                    if (valueReturned) return r;
                }

            if (Rules == null) throw new InvalidOperationException();
            return Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
        }

        public static CheckResult ConsiderCheckRuleSilently(String Name, params Object[] Arguments)
        {
            try
            {
                MudObject.SilentFlag = true;
                var r = ConsiderCheckRule(Name, Arguments);
                MudObject.SilentFlag = false;
                return r;
            }
            finally
            {
                MudObject.SilentFlag = false;
            }
        }

        public static void DiscoverRuleBooks(System.Reflection.Assembly In)
        {
            Rules = new RuleSet();

            foreach (var type in In.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(DeclaresRules)))
                {
                    var initializer = Activator.CreateInstance(type) as DeclaresRules;
                    initializer.InitializeRules();
                }
            }
        }
    }
}
