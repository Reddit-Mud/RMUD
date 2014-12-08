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

    public static class GlobalRules
    {
        private static RuleSet Rules = null;

        public static void DeclareActionRuleBook<T0>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0)).Description = Description;
        }

        public static void DeclareActionRuleBook<T0, T1>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0), typeof(T1)).Description = Description;
        }

        public static void DeclareActionRuleBook<T0, T1, T2>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
        }

        public static void DeclareValueRuleBook<T0, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0)).Description = Description;
        }

        public static void DeclareValueRuleBook<T0, T1, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1)).Description = Description;
        }

        public static void DeclareValueRuleBook<T0, T1, T2, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
        }

        public static RuleBuilder<T0, RuleResult> AddActionRule<T0>(String Name)
        {
            return Rules.AddRule<T0, RuleResult>(Name);
        }

        public static RuleBuilder<T0, T1, RuleResult> AddActionRule<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, RuleResult>(Name);
        }

        public static RuleBuilder<T0, T1, T2, RuleResult> AddActionRule<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RuleResult>(Name);
        }

        public static RuleBuilder<T0, RT> AddValueRule<T0, RT>(String Name)
        {
            return Rules.AddRule<T0, RT>(Name);
        }

        public static RuleBuilder<T0, T1, RT> AddValueRule<T0, T1, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, RT>(Name);
        }

        public static RuleBuilder<T0, T1, T2, RT> AddValueRule<T0, T1, T2, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RT>(Name);
        }

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
