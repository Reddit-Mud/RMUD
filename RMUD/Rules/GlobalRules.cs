using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface DeclaresRules
    {
        void InitializeGlobalRules();
    }

    public static partial class GlobalRules
    {
        internal static RuleSet Rules = null;
        internal static Client LogTo = null;

        public static void LogRules(Client To) { LogTo = To; }

        public static bool CheckGlobalRuleBookTypes(String Name, Type ResultType, params Type[] ArgumentTypes)
        {
            if (Rules == null) InitializeGlobalRuleBooks();

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;

            if (book.ResultType != ResultType) return false;
            return book.CheckArgumentTypes(ResultType, ArgumentTypes);
        }

        public static PerformResult ConsiderPerformRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = PerformResult.Continue;
            if (Object.Rules != null) r = Object.Rules.ConsiderActionRule(Name, Arguments);
            if (r == PerformResult.Continue)
            {
                if (Rules == null) InitializeGlobalRuleBooks();
                return Rules.ConsiderActionRule(Name, Arguments);
            }
            return r;
        }

        public static CheckResult ConsiderCheckRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = CheckResult.Continue;
            if (Object.Rules != null) r = Object.Rules.ConsiderCheckRule(Name, Arguments);
            if (r == CheckResult.Continue)
            {
                if (Rules == null) InitializeGlobalRuleBooks();
                return Rules.ConsiderCheckRule(Name, Arguments);
            }
            return r;
        }

        public static RT ConsiderValueRule<RT>(String Name, MudObject Object, params Object[] Arguments)
        {
            RT r = default(RT);
            bool valueReturned = false;
            if (Object.Rules != null) r = Object.Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
            if (!valueReturned)
            {
                if (Rules == null) InitializeGlobalRuleBooks();
                return Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
            }
            return r;
        }

        public static CheckResult ConsiderCheckRuleSilently(String Name, MudObject Object, params Object[] Arguments)
        {
            try
            {
                Mud.SilentFlag = true;
                var r = ConsiderCheckRule(Name, Object, Arguments);
                Mud.SilentFlag = false;
                return r;
            }
            finally
            {
                Mud.SilentFlag = false;
            }
        }

        private static void InitializeGlobalRuleBooks()
        {
            Rules = new RuleSet();

            foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(DeclaresRules)))
                {
                    var initializer = Activator.CreateInstance(type) as DeclaresRules;
                    initializer.InitializeGlobalRules();
                }
            }
        }
    }
}
