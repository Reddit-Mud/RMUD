//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{

	public class RuleBuilder<TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<TR> When(Func<bool> Clause)
        {
			if (Rule.WhenClause != null) 
			{
				var oldClause = Rule.WhenClause;
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(
					new Func<bool>(() => { 
						return Clause() && oldClause.Invoke(null);
					})
				);
			}
            else
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<TR> Do(Func<TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }

        public RuleBuilder<TR> ID(String ID)
        {
            Rule.ID = ID;
            return this;
        }

		public RuleBuilder<TR> First {
		get {
			Rule.Priority = RulePriority.First;
			return this;
		}}

		public RuleBuilder<TR> Last {
		get {
			Rule.Priority = RulePriority.Last;
			return this;
		}}
    }

	public class RuleBuilder<T0, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, TR> When(Func<T0, bool> Clause)
        {
			if (Rule.WhenClause != null) 
			{
				var oldClause = Rule.WhenClause;
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(
					new Func<T0, bool>((P0) => { 
						return Clause(P0) && oldClause.Invoke(new Object[]{P0});
					})
				);
			}
            else
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, TR> Do(Func<T0, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }

        public RuleBuilder<T0, TR> ID(String ID)
        {
            Rule.ID = ID;
            return this;
        }

		public RuleBuilder<T0, TR> First {
		get {
			Rule.Priority = RulePriority.First;
			return this;
		}}

		public RuleBuilder<T0, TR> Last {
		get {
			Rule.Priority = RulePriority.Last;
			return this;
		}}
    }

	public class RuleBuilder<T0, T1, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, T1, TR> When(Func<T0, T1, bool> Clause)
        {
			if (Rule.WhenClause != null) 
			{
				var oldClause = Rule.WhenClause;
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(
					new Func<T0, T1, bool>((P0, P1) => { 
						return Clause(P0, P1) && oldClause.Invoke(new Object[]{P0, P1});
					})
				);
			}
            else
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, TR> Do(Func<T0, T1, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }

        public RuleBuilder<T0, T1, TR> ID(String ID)
        {
            Rule.ID = ID;
            return this;
        }

		public RuleBuilder<T0, T1, TR> First {
		get {
			Rule.Priority = RulePriority.First;
			return this;
		}}

		public RuleBuilder<T0, T1, TR> Last {
		get {
			Rule.Priority = RulePriority.Last;
			return this;
		}}
    }

	public class RuleBuilder<T0, T1, T2, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, T1, T2, TR> When(Func<T0, T1, T2, bool> Clause)
        {
			if (Rule.WhenClause != null) 
			{
				var oldClause = Rule.WhenClause;
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(
					new Func<T0, T1, T2, bool>((P0, P1, P2) => { 
						return Clause(P0, P1, P2) && oldClause.Invoke(new Object[]{P0, P1, P2});
					})
				);
			}
            else
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, TR> Do(Func<T0, T1, T2, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }

        public RuleBuilder<T0, T1, T2, TR> ID(String ID)
        {
            Rule.ID = ID;
            return this;
        }

		public RuleBuilder<T0, T1, T2, TR> First {
		get {
			Rule.Priority = RulePriority.First;
			return this;
		}}

		public RuleBuilder<T0, T1, T2, TR> Last {
		get {
			Rule.Priority = RulePriority.Last;
			return this;
		}}
    }

	public class RuleBuilder<T0, T1, T2, T3, TR>
    {
        public Rule<TR> Rule;

        public RuleBuilder<T0, T1, T2, T3, TR> When(Func<T0, T1, T2, T3, bool> Clause)
        {
			if (Rule.WhenClause != null) 
			{
				var oldClause = Rule.WhenClause;
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(
					new Func<T0, T1, T2, T3, bool>((P0, P1, P2, P3) => { 
						return Clause(P0, P1, P2, P3) && oldClause.Invoke(new Object[]{P0, P1, P2, P3});
					})
				);
			}
            else
				Rule.WhenClause = RuleDelegateWrapper<bool>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, T3, TR> Do(Func<T0, T1, T2, T3, TR> Clause)
        {
            Rule.BodyClause = RuleDelegateWrapper<TR>.MakeWrapper(Clause);
            return this;
        }

        public RuleBuilder<T0, T1, T2, T3, TR> Name(String Name)
        {
            Rule.DescriptiveName = Name;
            return this;
        }

        public RuleBuilder<T0, T1, T2, T3, TR> ID(String ID)
        {
            Rule.ID = ID;
            return this;
        }

		public RuleBuilder<T0, T1, T2, T3, TR> First {
		get {
			Rule.Priority = RulePriority.First;
			return this;
		}}

		public RuleBuilder<T0, T1, T2, T3, TR> Last {
		get {
			Rule.Priority = RulePriority.Last;
			return this;
		}}
    }

}
