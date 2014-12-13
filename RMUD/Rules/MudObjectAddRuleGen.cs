//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public partial class MudObject
	{	
		public RuleBuilder<T0, RuleResult> AddActionRule<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, RuleResult>(Name);
		}

		public RuleBuilder<T0, RT> AddValueRule<T0, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, RT>(Name);
		}

		public RuleBuilder<T0, CheckRuleResult> AddCheckRule<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, CheckRuleResult>(Name);
		}
		public RuleBuilder<T0, T1, RuleResult> AddActionRule<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, RuleResult>(Name);
		}

		public RuleBuilder<T0, T1, RT> AddValueRule<T0, T1, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, RT>(Name);
		}

		public RuleBuilder<T0, T1, CheckRuleResult> AddCheckRule<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, CheckRuleResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, RuleResult> AddActionRule<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, RuleResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, RT> AddValueRule<T0, T1, T2, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, CheckRuleResult> AddCheckRule<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, CheckRuleResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, T3, RuleResult> AddActionRule<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, RuleResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, RT> AddValueRule<T0, T1, T2, T3, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, CheckRuleResult> AddCheckRule<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, CheckRuleResult>(Name);
		}
	}
}
