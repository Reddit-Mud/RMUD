using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class RuleBook
    {
        public String Name;
        public String Description;
        public List<Type> ArgumentTypes = new List<Type>();
        public Type ResultType;
        internal List<Rule> Rules = new List<Rule>();
        private bool NeedsSort = false;

        public bool CheckArgumentTypes(Type ResultType, params Type[] ArgTypes)
        {
            if (this.ResultType != ResultType) return false;
            if (ArgTypes.Length != ArgumentTypes.Count) return false;
            for (int i = 0; i < ArgTypes.Length; ++i)
                if (!ArgumentTypes[i].IsAssignableFrom(ArgTypes[i]))
                    return false;
            return true;
        }

        public virtual void AddRule(Rule Rule) { throw new NotImplementedException(); }
        
        protected void _addRule(Rule Rule)
        {
            Rules.Add(Rule);
            NeedsSort = true;
        }

        protected void SortRules()
        {
            var newList = new List<Rule>[]{ new List<Rule>(), new List<Rule>(), new List<Rule>() };
            foreach (var rule in Rules)
                newList[(int)rule.Priority].Add(rule);

            Rules.Clear();
            foreach (var sublist in newList)
                Rules.AddRange(sublist);

            NeedsSort = false;
        }

        public virtual void DeleteRule(String ID) { throw new NotImplementedException(); }
    }

    public class CheckRuleBook : RuleBook
    {
        public CheckRuleBook()
        {
            ResultType = typeof(CheckResult);
        }

        public CheckResult Consider(params Object[] Args)
        {
            SortRules();

            var rulesCopy = new List<Rule>(Rules);
            foreach (var _rule in rulesCopy)
            {
                var rule = _rule as Rule<CheckResult>;
                if (rule.WhenClause == null || rule.WhenClause.Invoke(Args))
                {
                    if (GlobalRules.LogTo != null)
                    {
                        GlobalRules.LogTo.Send(Name + "<" + String.Join(", ", ArgumentTypes.Select(t => t.Name)) + "> -> " + ResultType.Name + " : " + (String.IsNullOrEmpty(rule.DescriptiveName) ? "NONAME" : rule.DescriptiveName) + "\r\n");
                    }

                    var r = rule.BodyClause == null ? CheckResult.Continue : rule.BodyClause.Invoke(Args);
                    if (r != CheckResult.Continue) return r;
                }
            }
            return CheckResult.Continue;
        }

        public override void AddRule(Rule Rule)
        {
            if (!(Rule is Rule<CheckResult>)) throw new InvalidOperationException();
            _addRule(Rule);
        }

        public override void DeleteRule(string ID)
        {
            Rules.RemoveAll(r => r.ID == ID);
        }
    }


    public class ActionRuleBook : RuleBook
    {
        public ActionRuleBook()
        {
            ResultType = typeof(PerformResult);
        }

        public PerformResult Consider(params Object[] Args)
        {
            SortRules();

            var rulesCopy = new List<Rule>(Rules);
            foreach (var _rule in rulesCopy)
            {
                var rule = _rule as Rule<PerformResult>;
                if (rule.WhenClause == null || rule.WhenClause.Invoke(Args))
                {
                    if (GlobalRules.LogTo != null)
                    {
                        GlobalRules.LogTo.Send(Name + "<" + String.Join(", ", ArgumentTypes.Select(t => t.Name)) + "> -> " + ResultType.Name + " : " + (String.IsNullOrEmpty(rule.DescriptiveName) ? "NONAME" : rule.DescriptiveName) + "\r\n");
                    }

                    var r = rule.BodyClause == null ? PerformResult.Continue : rule.BodyClause.Invoke(Args);
                    if (r != PerformResult.Continue) return r;
                }
            }
            return PerformResult.Continue;
        }

        public override void AddRule(Rule Rule)
        {
            if (!(Rule is Rule<PerformResult>)) throw new InvalidOperationException();
            _addRule(Rule);
        }

        public override void DeleteRule(string ID)
        {
            Rules.RemoveAll(r => r.ID == ID);
        }
    }

    public class ValueRuleBook<RT> : RuleBook
    {
        public ValueRuleBook()
        {
            ResultType = typeof(RT);
        }

        public RT Consider(out bool ValueReturned, params Object[] Args)
        {
            SortRules();

            ValueReturned = false;
            var rulesCopy = new List<Rule>(Rules);
            foreach (var rule in rulesCopy)
                if (rule.WhenClause == null || rule.WhenClause.Invoke(Args))
                {
                    if (GlobalRules.LogTo != null)
                    {
                        GlobalRules.LogTo.Send(Name + "<" + String.Join(", ", ArgumentTypes.Select(t => t.Name)) + "> -> " + ResultType.Name + " : " + (String.IsNullOrEmpty(rule.DescriptiveName) ? "NONAME" : rule.DescriptiveName) + "\r\n");
                    }

                    ValueReturned = true;
                    return (rule as Rule<RT>).BodyClause.Invoke(Args);
                }
            return default(RT);
        }

        public override void AddRule(Rule Rule)
        {
            if (!(Rule is Rule<RT>)) throw new InvalidOperationException();
            _addRule(Rule);
        }

        public override void DeleteRule(string ID)
        {
            Rules.RemoveAll(r => r.ID == ID);
        }
    }
}
