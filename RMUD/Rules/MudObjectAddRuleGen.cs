//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public partial class MudObject
	{	
		public RuleBuilder<T0, PerformResult> AddPerformRule<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, PerformResult>(Name);
		}

		public RuleBuilder<T0, RT> AddValueRule<T0, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, RT>(Name);
		}

		public RuleBuilder<T0, CheckResult> AddCheckRule<T0>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, PerformResult> AddPerformRule<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, RT> AddValueRule<T0, T1, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, RT>(Name);
		}

		public RuleBuilder<T0, T1, CheckResult> AddCheckRule<T0, T1>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, PerformResult> AddPerformRule<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, RT> AddValueRule<T0, T1, T2, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, CheckResult> AddCheckRule<T0, T1, T2>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, CheckResult>(Name);
		}
		public RuleBuilder<T0, T1, T2, T3, PerformResult> AddPerformRule<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, PerformResult>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, RT> AddValueRule<T0, T1, T2, T3, RT>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
		}

		public RuleBuilder<T0, T1, T2, T3, CheckResult> AddCheckRule<T0, T1, T2, T3>(String Name)
		{
			if (Rules == null) Rules = new RuleSet();
			return Rules.AddRule<T0, T1, T2, T3, CheckResult>(Name);
		}
	}
}
