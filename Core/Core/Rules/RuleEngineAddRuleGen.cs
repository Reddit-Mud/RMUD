//This is generated code. Do not modify this file; modify the template that produces it.

using System;
using System.Collections.Generic;

namespace RMUD
{
	public partial class RuleEngine
	{	
		public void DeclarePerformRuleBook(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, 0).Description = Description;
		}
		
		public void DeclareValueRuleBook<RT>(String Name, String Description)
        {
		    Rules.FindOrCreateRuleBook<RT>(Name, 0).Description = Description;
        }

		public void DeclareCheckRuleBook(String Name, String Description)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, 0).Description = Description;
		}
				
        public RuleBuilder<PerformResult> Perform(String Name)
        {
			var rule = new Rule<PerformResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<PerformResult>(Name, 0).AddRule(rule); });
			return new RuleBuilder<PerformResult> { Rule = rule };
        }
		
        public RuleBuilder<RT> Value<RT>(String Name)
        {
			var rule = new Rule<RT>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<RT>(Name, 0).AddRule(rule); });
			return new RuleBuilder<RT> { Rule = rule };
        }

		public RuleBuilder<CheckResult> Check(String Name)
        {
			var rule = new Rule<CheckResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<CheckResult>(Name, 0).AddRule(rule); });
			return new RuleBuilder<CheckResult> { Rule = rule };
        }

		public void DeclarePerformRuleBook<T0>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, 1).Description = Description;
		}
		
		public void DeclareValueRuleBook<T0, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, 1).Description = Description;
		}

		public void DeclareCheckRuleBook<T0>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, 1).Description = Description;
		}
				
        public RuleBuilder<T0, PerformResult> Perform<T0>(String Name)
        {
			var rule = new Rule<PerformResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<PerformResult>(Name, 1).AddRule(rule); });
			return new RuleBuilder<T0, PerformResult> { Rule = rule };
        }
		
        public RuleBuilder<T0, RT> Value<T0, RT>(String Name)
        {
			var rule = new Rule<RT>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<RT>(Name, 1).AddRule(rule); });
			return new RuleBuilder<T0, RT> { Rule = rule };
        }

		public RuleBuilder<T0, CheckResult> Check<T0>(String Name)
        {
			var rule = new Rule<CheckResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<CheckResult>(Name, 1).AddRule(rule); });
			return new RuleBuilder<T0, CheckResult> { Rule = rule };
        }

		public void DeclarePerformRuleBook<T0, T1>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, 2).Description = Description;
		}
		
		public void DeclareValueRuleBook<T0, T1, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, 2).Description = Description;
		}

		public void DeclareCheckRuleBook<T0, T1>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, 2).Description = Description;
		}
				
        public RuleBuilder<T0, T1, PerformResult> Perform<T0, T1>(String Name)
        {
			var rule = new Rule<PerformResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<PerformResult>(Name, 2).AddRule(rule); });
			return new RuleBuilder<T0, T1, PerformResult> { Rule = rule };
        }
		
        public RuleBuilder<T0, T1, RT> Value<T0, T1, RT>(String Name)
        {
			var rule = new Rule<RT>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<RT>(Name, 2).AddRule(rule); });
			return new RuleBuilder<T0, T1, RT> { Rule = rule };
        }

		public RuleBuilder<T0, T1, CheckResult> Check<T0, T1>(String Name)
        {
			var rule = new Rule<CheckResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<CheckResult>(Name, 2).AddRule(rule); });
			return new RuleBuilder<T0, T1, CheckResult> { Rule = rule };
        }

		public void DeclarePerformRuleBook<T0, T1, T2>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, 3).Description = Description;
		}
		
		public void DeclareValueRuleBook<T0, T1, T2, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, 3).Description = Description;
		}

		public void DeclareCheckRuleBook<T0, T1, T2>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, 3).Description = Description;
		}
				
        public RuleBuilder<T0, T1, T2, PerformResult> Perform<T0, T1, T2>(String Name)
        {
			var rule = new Rule<PerformResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<PerformResult>(Name, 3).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, PerformResult> { Rule = rule };
        }
		
        public RuleBuilder<T0, T1, T2, RT> Value<T0, T1, T2, RT>(String Name)
        {
			var rule = new Rule<RT>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<RT>(Name, 3).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, RT> { Rule = rule };
        }

		public RuleBuilder<T0, T1, T2, CheckResult> Check<T0, T1, T2>(String Name)
        {
			var rule = new Rule<CheckResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<CheckResult>(Name, 3).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, CheckResult> { Rule = rule };
        }

		public void DeclarePerformRuleBook<T0, T1, T2, T3>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, 4).Description = Description;
		}
		
		public void DeclareValueRuleBook<T0, T1, T2, T3, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, 4).Description = Description;
		}

		public void DeclareCheckRuleBook<T0, T1, T2, T3>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, 4).Description = Description;
		}
				
        public RuleBuilder<T0, T1, T2, T3, PerformResult> Perform<T0, T1, T2, T3>(String Name)
        {
			var rule = new Rule<PerformResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<PerformResult>(Name, 4).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, T3, PerformResult> { Rule = rule };
        }
		
        public RuleBuilder<T0, T1, T2, T3, RT> Value<T0, T1, T2, T3, RT>(String Name)
        {
			var rule = new Rule<RT>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<RT>(Name, 4).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, T3, RT> { Rule = rule };
        }

		public RuleBuilder<T0, T1, T2, T3, CheckResult> Check<T0, T1, T2, T3>(String Name)
        {
			var rule = new Rule<CheckResult>();
			CreateNewRule(() => { Rules.FindOrCreateRuleBook<CheckResult>(Name, 4).AddRule(rule); });
			return new RuleBuilder<T0, T1, T2, T3, CheckResult> { Rule = rule };
        }

	}
}
