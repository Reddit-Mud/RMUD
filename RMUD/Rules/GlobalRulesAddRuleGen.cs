//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public static partial class GlobalRules
	{	
		public static void DeclareActionRuleBook<T0>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0)).Description = Description;
        }
				
        public static RuleBuilder<T0, RuleResult> AddActionRule<T0>(String Name)
        {
            return Rules.AddRule<T0, RuleResult>(Name);
        }
		
        public static RuleBuilder<T0, RT> AddValueRule<T0, RT>(String Name)
        {
            return Rules.AddRule<T0, RT>(Name);
        }

		public static void DeclareActionRuleBook<T0, T1>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0), typeof(T1)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1)).Description = Description;
        }
				
        public static RuleBuilder<T0, T1, RuleResult> AddActionRule<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, RuleResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, RT> AddValueRule<T0, T1, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, RT>(Name);
        }

		public static void DeclareActionRuleBook<T0, T1, T2>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, T2, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
        }
				
        public static RuleBuilder<T0, T1, T2, RuleResult> AddActionRule<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RuleResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, T2, RT> AddValueRule<T0, T1, T2, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RT>(Name);
        }

		public static void DeclareActionRuleBook<T0, T1, T2, T3>(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<RuleResult>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
		}
		
		public static void DeclareValueRuleBook<T0, T1, T2, T3, RT>(String Name, String Description)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
        }
				
        public static RuleBuilder<T0, T1, T2, T3, RuleResult> AddActionRule<T0, T1, T2, T3>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, RuleResult>(Name);
        }
		
        public static RuleBuilder<T0, T1, T2, T3, RT> AddValueRule<T0, T1, T2, T3, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
        }

	}
}
