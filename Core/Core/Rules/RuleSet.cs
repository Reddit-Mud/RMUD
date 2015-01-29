using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class RuleSet
    {
        public RuleEngine GlobalRules;
        public List<RuleBook> RuleBooks = new List<RuleBook>();

        internal RuleSet(RuleEngine GlobalRules)
        {
            this.GlobalRules = GlobalRules;
        }

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
                    if (typeof(RT) == typeof(PerformResult))
                        r = new PerformRuleBook(this) { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };
                    else if (typeof(RT) == typeof(CheckResult))
                        r = new CheckRuleBook(this) { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };
                    else
                        r = new ValueRuleBook<RT>(this) { Name = Name, ArgumentTypes = new List<Type>(ArgumentTypes) };

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

        internal RT ConsiderValueRule<RT>(String Name, out bool ValueReturned, params Object[] Args)
        {
            ValueReturned = false;
            var book = FindRuleBook(Name);
            if (book != null)
            {
                //if (!book.CheckArgumentTypes(typeof(RT), Args.Select(o => o.GetType()).ToArray()))
                //    throw new InvalidOperationException();
                var valueBook = book as ValueRuleBook<RT>;
                if (valueBook == null) throw new InvalidOperationException();
                return valueBook.Consider(out ValueReturned, Args);
            }
            return default(RT);
        }

        internal PerformResult ConsiderPerformRule(String Name, params Object[] Args)
        {
            var book = FindRuleBook(Name);
            if (book != null)
            {
                //if (!book.CheckArgumentTypes(typeof(PerformResult), Args.Select(o => o.GetType()).ToArray()))
                //    throw new InvalidOperationException();
                var actionBook = book as PerformRuleBook;
                if (actionBook == null) throw new InvalidOperationException();
                return actionBook.Consider(Args);
            }
            return PerformResult.Continue;
        }

        internal CheckResult ConsiderCheckRule(String Name, params Object[] Args)
        {
            var book = FindRuleBook(Name);
            if (book != null)
            {
                //if (!book.CheckArgumentTypes(typeof(CheckResult), Args.Select(o => o.GetType()).ToArray()))
                //    throw new InvalidOperationException();
                var actionBook = book as CheckRuleBook;
                if (actionBook == null) throw new InvalidOperationException();
                return actionBook.Consider(Args);
            }
            return CheckResult.Continue;
        }
    }
}
