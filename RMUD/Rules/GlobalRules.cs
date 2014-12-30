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

        public static PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            foreach (var arg in Arguments)
                if (arg is MudObject && (arg as MudObject).Rules != null)
                    if ((arg as MudObject).Rules.ConsiderPerformRule(Name, Arguments) == PerformResult.Stop)
                        return PerformResult.Stop;

            if (Rules == null) InitializeGlobalRuleBooks();
            return Rules.ConsiderPerformRule(Name, Arguments);
        }

        public static CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            foreach (var arg in Arguments)
                if (arg is MudObject && (arg as MudObject).Rules != null)
                {
                    var r = (arg as MudObject).Rules.ConsiderCheckRule(Name, Arguments);
                    if (r != CheckResult.Continue) return r;
                }

            if (Rules == null) InitializeGlobalRuleBooks();
            return Rules.ConsiderCheckRule(Name, Arguments);
        }

        public static RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            bool valueReturned = false;

            foreach (var arg in Arguments)
                if (arg is MudObject && (arg as MudObject).Rules != null)
                {
                    var r = (arg as MudObject).Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
                    if (valueReturned) return r;
                }

            if (Rules == null) InitializeGlobalRuleBooks();
            return Rules.ConsiderValueRule<RT>(Name, out valueReturned, Arguments);
        }

        public static CheckResult ConsiderCheckRuleSilently(String Name, params Object[] Arguments)
        {
            try
            {
                Mud.SilentFlag = true;
                var r = ConsiderCheckRule(Name, Arguments);
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
