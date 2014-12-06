using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RuleResult
    {
        Allow = 0,
        Default = 1,
        Continue = 1,
        Stop = 2,
        Disallow = 2,
    }

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
        public RuleDelegateWrapper<bool> WhenClause;
        public RuleDelegateWrapper<RuleResult> BodyClause;
    }

    public class RuleSet
    {
        public List<RuleBook> RuleBooks = new List<RuleBook>();

        internal RuleBook FindRuleBook(String Name)
        {
            return RuleBooks.FirstOrDefault(r => r.Name == Name);
        }

        internal RuleBook FindOrCreateRuleBook(String Name, params Type[] ArgumentTypes)
        {
            var r = FindRuleBook(Name);
            if (r == null)
            {
                if (GlobalRuleBooks.CheckGlobalRuleBookArgumentTypes(Name, ArgumentTypes))
                {
                    r = new RuleBook { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };
                    RuleBooks.Add(r);
                }
                else
                    throw new InvalidOperationException("Local rulebook definition does not match global rulebook");
            }
            else if (!r.CheckArgumentTypes(ArgumentTypes))
            {
                throw new InvalidOperationException("Rule inconsistent with existing rulebook");
            }
            return r;
        }

        public RuleBuilder<T0> AddRule<T0>(String Name)
        {
            var rule = new Rule();
            FindOrCreateRuleBook(Name, typeof(T0)).Rules.Add(rule);
            return new RuleBuilder<T0> { Rule = rule };
        }

        public RuleBuilder<T0, T1> AddRule<T0, T1>(String Name)
        {
            var rule = new Rule();
            FindOrCreateRuleBook(Name, typeof(T0), typeof(T1)).Rules.Add(rule);
            return new RuleBuilder<T0, T1> { Rule = rule };
        }

        public RuleBuilder<T0, T1, T2> AddRule<T0, T1, T2>(String Name)
        {
            var rule = new Rule();
            FindOrCreateRuleBook(Name, typeof(T0), typeof(T1), typeof(T2)).Rules.Add(rule);
            return new RuleBuilder<T0, T1, T2> { Rule = rule };
        }
        
        public RuleResult ConsiderRule(String Name, params Object[] Args)
        {
            var book = FindRuleBook(Name);
            if (book != null && book.CheckArgumentTypes(Args.Select(o => o.GetType()).ToArray()))
                return book.Consider(Args);
            return RuleResult.Default;
        }
    }

    public class RuleBuilder<T0>
    {
        public Rule Rule;
        
        public RuleBuilder<T0> When(Func<T0, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0> Do(Func<T0, RuleResult> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, RuleResult>.MakeWrapper(Clause);
            return this;
        }
    }
    public class RuleBuilder<T0, T1>
    {
        public Rule Rule;

        public RuleBuilder<T0, T1> When(Func<T0, T1, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, T1, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1> Do(Func<T0, T1, RuleResult> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, T1, RuleResult>.MakeWrapper(Clause);
            return this;
        }
    }
    public class RuleBuilder<T0, T1, T2>
    {
        public Rule Rule;

        public RuleBuilder<T0, T1, T2> When(Func<T0, T1, T2, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, T1, T2, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2> Do(Func<T0, T1, T2, RuleResult> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, T1, T2, RuleResult>.MakeWrapper(Clause);
            return this;
        }
    }

    public class RuleBook
    {
        public String Name;
        public List<Rule> Rules = new List<Rule>();
        public List<Type> ArgumentTypes = new List<Type>();

        public bool CheckArgumentTypes(params Type[] ArgTypes)
        {
            if (ArgTypes.Length != ArgumentTypes.Count) return false;
            for (int i = 0; i < ArgTypes.Length; ++i)
                if (!ArgumentTypes[i].IsAssignableFrom(ArgTypes[i]))
                    return false;
            return true;
        }

        public RuleResult Consider(params Object[] Args)
        {
            foreach (var rule in Rules)
            {
                if (rule.WhenClause == null || rule.WhenClause.Invoke(Args))
                {
                    var r = rule.BodyClause ==  null ? RuleResult.Default : rule.BodyClause.Invoke(Args);
                    if (r != RuleResult.Continue) return r;
                }
            }
            return RuleResult.Default;
        }
    }

    public interface ActionRules
    {
        void CreateGlobalRules();
    }

    public static class GlobalRuleBooks
    {
        private static RuleSet Rules = null;

        public static void DeclareRuleBook<T0>(String Name)
        {
            Rules.RuleBooks.Add(new RuleBook { Name = Name, ArgumentTypes = new List<Type>(new Type[] { typeof(T0) }) });
        }

        public static void DeclareRuleBook<T0, T1>(String Name)
        {
            Rules.RuleBooks.Add(new RuleBook { Name = Name, ArgumentTypes = new List<Type>(new Type[] { typeof(T0), typeof(T1) }) });
        }

        public static void DeclareRuleBook<T0, T1, T2>(String Name)
        {
            Rules.RuleBooks.Add(new RuleBook { Name = Name, ArgumentTypes = new List<Type>(new Type[] { typeof(T0), typeof(T1), typeof(T2) }) });
        }

        public static RuleBuilder<T0> AddRule<T0>(String Name)
        {
            return Rules.AddRule<T0>(Name);
        }

        public static RuleBuilder<T0, T1> AddRule<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1>(Name);
        }

        public static RuleBuilder<T0, T1, T2> AddRule<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2>(Name);
        }

        public static bool CheckGlobalRuleBookArgumentTypes(String Name, params Type[] ArgumentTypes)
        {
            if (Rules == null) InitializeGlobalRuleBooks();

            var book = Rules.FindRuleBook(Name);
            if (book == null) return true;
            return book.CheckArgumentTypes(ArgumentTypes);
        }

        public static RuleResult ConsiderGlobalRuleBook(String Name, params Object[] Arguments)
        {
            if (Rules == null) InitializeGlobalRuleBooks();
            
            return Rules.ConsiderRule(Name, Arguments);
        }

        public static RuleResult ConsiderRuleFamily(String Name, MudObject Object, params Object[] Arguments)
        {
            var r = RuleResult.Continue;
            if (Object.Rules != null) r = Object.Rules.ConsiderRule(Name, Arguments);
            if (r == RuleResult.Continue) r = ConsiderGlobalRuleBook(Name, Arguments);
            return r;
        }

        public static RuleResult ConsiderRuleFamilySilently(String Name, MudObject Object, params Object[] Arguments)
        {
            try
            {
                Mud.SilentFlag = true;
                var r = ConsiderRuleFamily(Name, Object, Arguments);
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
                if (type.GetInterfaces().Contains(typeof(ActionRules)))
                {
                    var initializer = Activator.CreateInstance(type) as ActionRules;
                    initializer.CreateGlobalRules();
                }
            }
        }
    }
}
