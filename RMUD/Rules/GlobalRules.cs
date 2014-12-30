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

        public static CheckResult IsVisibleTo(MudObject Actor, MudObject Item)
        {
            if (!Mud.IsVisibleTo(Actor, Item))
            {
                Mud.SendMessage(Actor, "That doesn't seem to be here any more.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }

        public static CheckResult IsHolding(MudObject Actor, MudObject Item)
        {
            if (!Mud.ObjectContainsObject(Actor, Item))
            {
                Mud.SendMessage(Actor, "You don't have that.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }

        public static void DeleteRule(String RuleBookName, String RuleID)
        {
            Rules.DeleteRule(RuleBookName, RuleID);
        }

        public static PerformResult ConsiderPerformRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = PerformResult.Continue;
            if (Object != null && Object.Rules != null) r = Object.Rules.ConsiderPerformRule(Name, Arguments);
            if (r == PerformResult.Continue)
            {
                if (Rules == null) InitializeGlobalRuleBooks();
                return Rules.ConsiderPerformRule(Name, Arguments);
            }
            return r;
        }

        public static CheckResult ConsiderCheckRule(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = CheckResult.Continue;
            if (Object != null && Object.Rules != null) r = Object.Rules.ConsiderCheckRule(Name, Arguments);
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
            if (Object != null && Object.Rules != null) r = Object.Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
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
