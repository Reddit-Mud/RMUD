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

        public RuleSet(RuleEngine GlobalRules)
        {
            this.GlobalRules = GlobalRules;
        }

        public RuleBook FindRuleBook(String Name)
        {
            return RuleBooks.FirstOrDefault(r => r.Name == Name);
        }

        internal RuleBook FindOrCreateRuleBook<RT>(String Name, int ArgCount)
        {
            var r = FindRuleBook(Name);

            if (r == null)
            {
                if (typeof(RT) == typeof(PerformResult))
                    r = new PerformRuleBook(this) { Name = Name, ArgumentCount = ArgCount };
                else if (typeof(RT) == typeof(CheckResult))
                    r = new CheckRuleBook(this) { Name = Name, ArgumentCount = ArgCount };
                else
                    r = new ValueRuleBook<RT>(this) { Name = Name, ArgumentCount = ArgCount };

                RuleBooks.Add(r);
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

        public RT ConsiderValueRule<RT>(String Name, out bool ValueReturned, params Object[] Args)
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

        public PerformResult ConsiderPerformRule(String Name, params Object[] Args)
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

        public CheckResult ConsiderCheckRule(String Name, params Object[] Args)
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
