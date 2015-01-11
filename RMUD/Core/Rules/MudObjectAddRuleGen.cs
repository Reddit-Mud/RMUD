//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public partial class MudObject
	{	
		public RuleBuilder<PerformResult> Perform(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<PerformResult>(Name);
		}

		public RuleBuilder<RT> Value<RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<RT>(Name);
		}

		public RuleBuilder<CheckResult> Check(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<CheckResult>(Name);
		}

		public RuleBuilder<T0, PerformResult> Perform<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, PerformResult>(Name);
		}

		public RuleBuilder<T0, RT> Value<T0, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, RT>(Name);
		}

		public RuleBuilder<T0, CheckResult> Check<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, PerformResult> Perform<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, RT> Value<T0, T1, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, RT>(Name);
		}

		public RuleBuilder<T0, T1, CheckResult> Check<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, PerformResult> Perform<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, RT> Value<T0, T1, T2, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, CheckResult> Check<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, T3, PerformResult> Perform<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, T3, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, RT> Value<T0, T1, T2, T3, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, CheckResult> Check<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet(GlobalRules);
			return Rules.AddRule<T0, T1, T2, T3, CheckResult>(Name);
		}
	}
}
