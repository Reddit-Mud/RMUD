//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public static partial class GlobalRules
	{	
		public static void DeclarePerformRuleBook<T0>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0)).Description = Description;
        }

		public static void DeclareCheckRuleBook<T0>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0)).Description = Description;
		}
				
        public static RuleBuilder<T0, PerformResult> AddPerformRule<T0>(String Name)
        {
            return Rules.AddRule<T0, PerformResult>(Name);
        }
		
        public static RuleBuilder<T0, RT> AddValueRule<T0, RT>(String Name)
        {
            return Rules.AddRule<T0, RT>(Name);
        }

		public static RuleBuilder<T0, CheckResult> AddCheckRule<T0>(String Name)
        {
            return Rules.AddRule<T0, CheckResult>(Name);
        }

		public static void DeclarePerformRuleBook<T0, T1>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1)).Description = Description;
        }

		public static void DeclareCheckRuleBook<T0, T1>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1)).Description = Description;
		}
				
        public static RuleBuilder<T0, T1, PerformResult> AddPerformRule<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, PerformResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, RT> AddValueRule<T0, T1, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, RT>(Name);
        }

		public static RuleBuilder<T0, T1, CheckResult> AddCheckRule<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, CheckResult>(Name);
        }

		public static void DeclarePerformRuleBook<T0, T1, T2>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, T2, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
        }

		public static void DeclareCheckRuleBook<T0, T1, T2>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
		}
				
        public static RuleBuilder<T0, T1, T2, PerformResult> AddPerformRule<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, PerformResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, T2, RT> AddValueRule<T0, T1, T2, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RT>(Name);
        }

		public static RuleBuilder<T0, T1, T2, CheckResult> AddCheckRule<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, CheckResult>(Name);
        }

		public static void DeclarePerformRuleBook<T0, T1, T2, T3>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, T2, T3, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
        }

		public static void DeclareCheckRuleBook<T0, T1, T2, T3>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
		}
				
        public static RuleBuilder<T0, T1, T2, T3, PerformResult> AddPerformRule<T0, T1, T2, T3>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, PerformResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, T2, T3, RT> AddValueRule<T0, T1, T2, T3, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
        }

		public static RuleBuilder<T0, T1, T2, T3, CheckResult> AddCheckRule<T0, T1, T2, T3>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, CheckResult>(Name);
        }

	}
}
