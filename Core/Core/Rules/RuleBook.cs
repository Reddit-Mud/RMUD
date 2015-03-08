using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RuleComparer : System.Collections.Generic.IComparer<Rule>
    {
        public int Compare(Rule x, Rule y)
        {
            var typesA = x.GetArgumentTypes();
            var typesB = y.GetArgumentTypes();
            if (typesA.Length != typesB.Length) return 0;

            for (int i = 0; i < typesA.Length; ++i)
            {
                if (typesA[i] != typesB[i])
                {
                    if (typesA[i].IsSubclassOf(typesB[i])) return -1;
                    else if (typesB[i].IsSubclassOf(typesA[i])) return 1;
                }
            }

            return 0;
        }
    }

    public class RuleBook
    {
        public String Name;
        public String Description;
        public List<Type> ArgumentTypes = new List<Type>();
        public Type ResultType;
        public RuleSet Owner;
        public List<Rule> Rules = new List<Rule>();
        private bool NeedsSort = false;

        public RuleBook(RuleSet Owner)
        {
            this.Owner = Owner;
        }

        public bool CheckArgumentTypes(Type ResultType, params Type[] ArgTypes)
        {
            if (this.ResultType != ResultType) return false;
            if (ArgTypes.Length != ArgumentTypes.Count) return false;
            for (int i = 0; i < ArgTypes.Length; ++i)
                if (!ArgumentTypes[i].IsAssignableFrom(ArgTypes[i]))
                    return false;
            return true;
        }

        public virtual void CheckRule(Rule Rule) { throw new NotImplementedException(); }
        
        public void AddRule(Rule Rule)
        {
            CheckRule(Rule);
            Rules.Add(Rule);
            NeedsSort = true;
        }

        protected void SortRules()
        {
            if (!NeedsSort) return;

            var newList = new List<Rule>[]{ new List<Rule>(), new List<Rule>(), new List<Rule>() };
            foreach (var rule in Rules)
                if (rule.Priority != RulePriority.Delete)
                    newList[(int)rule.Priority].Add(rule);

            Rules.Clear();
            foreach (var sublist in newList)
            {
                sublist.Sort(new RuleComparer());
                Rules.AddRange(sublist);
            }

            NeedsSort = false;
        }

        public void DeleteRule(string ID)
        {
            foreach (var rule in Rules)
                if (rule.ID == ID) rule.Priority = RulePriority.Delete;
            NeedsSort = true;
        }

        protected void LogRule(Rule Rule)
        {
            //if (Owner.GlobalRules.LogTo != null && Owner.GlobalRules.LogTo.ConnectedClient != null)
            //{
            //    Owner.GlobalRules.LogTo.ConnectedClient.Send(Name + "<" + String.Join(", ", ArgumentTypes.Select(t => t.Name)) + "> -> " + ResultType.Name + " : " + (String.IsNullOrEmpty(Rule.DescriptiveName) ? "NONAME" : Rule.DescriptiveName) + "\r\n");
            //}
        }
    }

    public class CheckRuleBook : RuleBook
    {
        public CheckRuleBook(RuleSet Owner)
            : base(Owner)
        {
            ResultType = typeof(CheckResult);
        }

        public CheckResult Consider(params Object[] Args)
        {
            SortRules();

            foreach (var _rule in Rules)
            {
                var rule = _rule as Rule<CheckResult>;
                if (rule.AreArgumentsCompatible(Args) && rule.CheckWhenClause(Args))
                {
                    LogRule(rule);
                    var r = rule.BodyClause == null ? CheckResult.Continue : rule.BodyClause.Invoke(Args);
                    if (r != CheckResult.Continue) return r;
                }
            }
            return CheckResult.Continue;
        }

        public override void CheckRule(Rule Rule)
        {
            if (!(Rule is Rule<CheckResult>)) throw new InvalidOperationException();
        }
    }

    public class PerformRuleBook : RuleBook
    {
        public PerformRuleBook(RuleSet Owner) : base(Owner)
        {
            ResultType = typeof(PerformResult);
        }

        public PerformResult Consider(params Object[] Args)
        {
            SortRules();

            foreach (var _rule in Rules)
            {
                var rule = _rule as Rule<PerformResult>;
                if (rule.AreArgumentsCompatible(Args) && rule.CheckWhenClause(Args))
                {
                    LogRule(rule);
                    var r = rule.BodyClause == null ? PerformResult.Continue : rule.BodyClause.Invoke(Args);
                    if (r != PerformResult.Continue) return r;
                }
            }
            return PerformResult.Continue;
        }

        public override void CheckRule(Rule Rule)
        {
            if (!(Rule is Rule<PerformResult>)) throw new InvalidOperationException();
        }
    }

    public class ValueRuleBook<RT> : RuleBook
    {
        public ValueRuleBook(RuleSet Owner)
            : base(Owner)
        {
            ResultType = typeof(RT);
        }

        public RT Consider(out bool ValueReturned, params Object[] Args)
        {
            SortRules();

            ValueReturned = false;
            foreach (var _rule in Rules)
            {
                var rule = _rule as Rule<RT>;
                if (rule.AreArgumentsCompatible(Args) && rule.CheckWhenClause(Args))
                {
                    LogRule(rule);
                    ValueReturned = true;
                    return rule.BodyClause.Invoke(Args);
                }
            }
            return default(RT);
        }

        public override void CheckRule(Rule Rule)
        {
            if (!(Rule is Rule<RT>)) throw new InvalidOperationException();
        }
    }
}
