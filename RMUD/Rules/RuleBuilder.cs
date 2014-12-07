using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
     public class RuleBuilder<T0, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, TR> When(Func<T0, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, TR> Do(Func<T0, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }
    }

    public class RuleBuilder<T0, T1, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, T1, TR> When(Func<T0, T1, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, T1, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, TR> Do(Func<T0, T1, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, T1, TR>.MakeWrapper(Clause);
            return this;
        }
        
        public RuleBuilder<T0, T1, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }
    }

    public class RuleBuilder<T0, T1, T2, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, T1, T2, TR> When(Func<T0, T1, T2, bool> Clause)
        {
            Rule.WhenClause = RuleDelegateWrapper<T0, T1, T2, bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, TR> Do(Func<T0, T1, T2, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<T0, T1, T2, TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }
    }
}
