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

        public static RuleResult ConsiderActionRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = RuleResult.Continue;
            if (Object.Rules != null) r = Object.Rules.ConsiderActionRule(Name, Arguments);
            if (r == RuleResult.Continue)
            {
                if (Rules == null) InitializeGlobalRuleBooks();
                return Rules.ConsiderActionRule(Name, Arguments);
            }
            return r;
        }

        public static CheckRuleResult ConsiderCheckRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = CheckRuleResult.Continue;
            if (Object.Rules != null) r = Object.Rules.ConsiderCheckRule(Name, Arguments);
            if (r == CheckRuleResult.Continue)
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

        public static RuleResult ConsiderActionRuleSilently(String Name, MudObject Object, params Object[] Arguments)
        {
            try
            {
                Mud.SilentFlag = true;
                var r = ConsiderActionRule(Name, Object, Arguments);
                Mud.SilentFlag = false;
                return r;
            }
            finally
            {
                Mud.SilentFlag = false;
            }
        }

        public static CheckRuleResult ConsiderCheckRuleSilently(String Name, MudObject Object, params Object[] Arguments)
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
