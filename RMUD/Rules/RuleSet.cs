using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RuleSet
    {
        public List<RuleBook> RuleBooks = new List<RuleBook>();

        internal RuleBook FindRuleBook(String Name)
        {
            return RuleBooks.FirstOrDefault(r => r.Name == Name);
        }

        internal RuleBook FindOrCreateRuleBook<RT>(String Name, params Type[] ArgumentTypes)
        {
            var r = FindRuleBook(Name);

            if (r == null)
            {
                if (GlobalRules.CheckGlobalRuleBookTypes(Name, typeof(RT), ArgumentTypes))
                {
                    if (typeof(RT) == typeof(RuleResult))
                        r = new ActionRuleBook { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };
                    else
                        r = new ValueRuleBook<RT> { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };

                    RuleBooks.Add(r);
                }
                else
                    throw new InvalidOperationException("Local rulebook definition does not match global rulebook");
            }
            else if (!r.CheckArgumentTypes(typeof(RT), ArgumentTypes))
            {
                throw new InvalidOperationException("Rule inconsistent with existing rulebook");
            }

            return r;
        }

        public RuleBuilder<T0, RT> AddRule<T0, RT>(String Name)
        {
            var rule = new Rule<RT>();
            FindOrCreateRuleBook<RT>(Name, typeof(T0)).AddRule(rule);
            return new RuleBuilder<T0, RT> { Rule = rule };
        }

        public RuleBuilder<T0, T1, RT> AddRule<T0, T1, RT>(String Name)
        {
            var rule = new Rule<RT>();
            FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1)).AddRule(rule);
            return new RuleBuilder<T0, T1, RT> { Rule = rule };
        }

        public RuleBuilder<T0, T1, T2, RT> AddRule<T0, T1, T2, RT>(String Name)
        {
            var rule = new Rule<RT>();
            FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2)).AddRule(rule);
            return new RuleBuilder<T0, T1, T2, RT> { Rule = rule };
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            var book = FindRuleBook(RuleBookName);
            if (book != null) book.DeleteRule(RuleID);
        }

        public void DeleteAll(String RuleID)
        {
            foreach (var book in RuleBooks)
                book.DeleteRule(RuleID);
        }

        public RuleBuilder<T0, T1, T2, T3, RT> AddRule<T0, T1, T2, T3, RT>(String Name)
        {
            var rule = new Rule<RT>();
            FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).AddRule(rule);
            return new RuleBuilder<T0, T1, T2, T3, RT> { Rule = rule };
        }
        
        public RT ConsiderValueRule<RT>(String Name, out bool ValueReturned, params Object[] Args)
        {
            ValueReturned = false;
            var book = FindRuleBook(Name);
            if (book != null)
            {
                if (!book.CheckArgumentTypes(typeof(RT), Args.Select(o => o.GetType()).ToArray()))
                    throw new InvalidOperationException();
                var valueBook = book as ValueRuleBook<RT>;
                if (valueBook == null) throw new InvalidOperationException();
                return valueBook.Consider(out ValueReturned, Args);
            }
            return default(RT);
        }

        public RuleResult ConsiderActionRule(String Name, params Object[] Args)
        {
            var book = FindRuleBook(Name);
            if (book != null)
            {
                if (!book.CheckArgumentTypes(typeof(RuleResult), Args.Select(o => o.GetType()).ToArray()))
                    throw new InvalidOperationException();
                var actionBook = book as ActionRuleBook;
                if (actionBook == null) throw new InvalidOperationException();
                return actionBook.Consider(Args);
            }
            return RuleResult.Default;
        }
    }
}
