using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RuleDelegateWrapper<TR>
    {
        public virtual TR Invoke(Object[] Arguments)
        {
            throw new NotImplementedException();
        }

        public static RuleDelegateWrapper<TR> MakeWrapper<T0>(Func<T0, TR> Delegate)
        {
            return new RuleDelegateWrapper<T0, TR> { Delegate = Delegate };
        }

        public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1>(Func<T0, T1, TR> Delegate)
        {
            return new RuleDelegateWrapper<T0, T1, TR> { Delegate = Delegate };
        }

        public static RuleDelegateWrapper<TR> MakeWrapper<T0, T1, T2>(Func<T0, T1, T2, TR> Delegate)
        {
            return new RuleDelegateWrapper<T0, T1, T2, TR> { Delegate = Delegate };
        }
    }

    public class RuleDelegateWrapper<T0, TR> : RuleDelegateWrapper<TR>
    {
        internal Func<T0, TR> Delegate;

        public override TR Invoke(Object[] Arguments)
        {
            return (TR)Delegate.DynamicInvoke(Arguments);
        }
    }

    public class RuleDelegateWrapper<T0, T1, TR> : RuleDelegateWrapper<TR>
    {
        internal Func<T0, T1, TR> Delegate;

        public override TR Invoke(Object[] Arguments)
        {
            return (TR)Delegate.DynamicInvoke(Arguments);
        }
    }

    public class RuleDelegateWrapper<T0, T1, T2, TR> : RuleDelegateWrapper<TR>
    {
        internal Func<T0, T1, T2, TR> Delegate;

        public override TR Invoke(Object[] Arguments)
        {
            return (TR)Delegate.DynamicInvoke(Arguments);
        }
    }

    public class Rule
    {
        public String Name;
        public List<Type> ArgumentTypes;
        public RuleDelegateWrapper<CheckRuleResponse> CheckRule;
        public RuleDelegateWrapper<RuleHandlerFollowUp> HandleRule;

        public CheckRuleResponse Check(params Object[] Args)
        {
            if (CheckArguments(Args)) return CheckRule.Invoke(Args);
            else return CheckRuleResponse.Allow;
        }

        public RuleHandlerFollowUp Handle(params Object[] Args)
        {
            if (CheckArguments(Args)) return HandleRule.Invoke(Args);
            else return RuleHandlerFollowUp.Continue;
        }

        public bool CheckArguments(Object[] Args)
        {
            if (Args.Length != ArgumentTypes.Count) return false;
            for (int i = 0; i < Args.Length; ++i)
                if (!ArgumentTypes[i].IsAssignableFrom(Args[i].GetType()))
                    return false;
            return true;
        }

        public static Rule CreateRule<T0>(String Name, Func<T0, CheckRuleResponse> CheckRule, Func<T0, RuleHandlerFollowUp> HandleRule)
        {
            var r = new Rule { Name = Name };
            if (CheckRule == null) CheckRule = (a) => CheckRuleResponse.Allow;
            if (HandleRule == null) HandleRule = (a) => RuleHandlerFollowUp.Continue;

            r.CheckRule = RuleDelegateWrapper<CheckRuleResponse>.MakeWrapper(CheckRule);
            r.HandleRule = RuleDelegateWrapper<RuleHandlerFollowUp>.MakeWrapper(HandleRule);

            r.ArgumentTypes = new List<Type>(new Type[] { typeof(T0) });
            return r;
        }

        public static Rule CreateRule<T0, T1>(String Name, Func<T0, T1, CheckRuleResponse> CheckRule, Func<T0, T1, RuleHandlerFollowUp> HandleRule)
        {
            var r = new Rule { Name = Name };
            if (CheckRule == null) CheckRule = (a, b) => CheckRuleResponse.Allow;
            if (HandleRule == null) HandleRule = (a, b) => RuleHandlerFollowUp.Continue;

            r.CheckRule = RuleDelegateWrapper<CheckRuleResponse>.MakeWrapper(CheckRule);
            r.HandleRule = RuleDelegateWrapper<RuleHandlerFollowUp>.MakeWrapper(HandleRule);

            r.ArgumentTypes = new List<Type>(new Type[] { typeof(T0), typeof(T1) });
            return r;
        }

        public static Rule CreateRule<T0, T1, T2>(String Name, Func<T0, T1, T2, CheckRuleResponse> CheckRule, Func<T0, T1, T2, RuleHandlerFollowUp> HandleRule)
        {
            var r = new Rule { Name = Name };
            if (CheckRule == null) CheckRule = (a, b, c) => CheckRuleResponse.Allow;
            if (HandleRule == null) HandleRule = (a, b, c) => RuleHandlerFollowUp.Continue;

            r.CheckRule = RuleDelegateWrapper<CheckRuleResponse>.MakeWrapper(CheckRule);
            r.HandleRule = RuleDelegateWrapper<RuleHandlerFollowUp>.MakeWrapper(HandleRule);

            r.ArgumentTypes = new List<Type>(new Type[] { typeof(T0), typeof(T1), typeof(T2) });
            return r;
        }
    }

    public class RuleSet
    {
        public List<Rule> Rules = new List<Rule>();

        public void Add<T0>(String Name, Func<T0, CheckRuleResponse> CheckRule, Func<T0, RuleHandlerFollowUp> HandleRule)
        {
            Rules.Add(Rule.CreateRule(Name, CheckRule, HandleRule));
        }

        public void Add<T0, T1>(String Name, Func<T0, T1, CheckRuleResponse> CheckRule, Func<T0, T1, RuleHandlerFollowUp> HandleRule)
        {
            Rules.Add(Rule.CreateRule(Name, CheckRule, HandleRule));
        }

        public void Add<T0, T1, T2>(String Name, Func<T0, T1, T2, CheckRuleResponse> CheckRule, Func<T0, T1, T2, RuleHandlerFollowUp> HandleRule)
        {
            Rules.Add(Rule.CreateRule(Name, CheckRule, HandleRule));
        }

        public CheckRuleResponse Check(String Name, params Object[] Args)
        {
            var matchingRule = Rules.FirstOrDefault(r => r.Name == Name);
            if (matchingRule != null) return matchingRule.Check(Args);
            return CheckRuleResponse.Allow;
        }

        public RuleHandlerFollowUp Handle(String Name, params Object[] Args)
        {
            var matchingRule = Rules.FirstOrDefault(r => r.Name == Name);
            if (matchingRule != null) return matchingRule.Handle(Args);
            return RuleHandlerFollowUp.Continue;
        }
    }
}
