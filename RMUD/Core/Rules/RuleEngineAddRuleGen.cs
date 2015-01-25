//This is generated code. Do not modify this file; modify the template that produces it.

using System;
using System.Collections.Generic;

namespace RMUD
{
	public partial class RuleEngine
	{	
		public void DeclarePerformRuleBook(String Name, String Description)
		{
		    MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Perform);
			Rules.FindOrCreateRuleBook<PerformResult>(Name).Description = Description;
		}
		
		public void DeclareValueRuleBook<RT>(String Name, String Description)
        {
		    MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Value, typeof(RT).Name + " value");
            Rules.FindOrCreateRuleBook<RT>(Name).Description = Description;
        }

		public void DeclareCheckRuleBook(String Name, String Description)
		{
		    MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Check);
			Rules.FindOrCreateRuleBook<CheckResult>(Name).Description = Description;
		}
				
        public RuleBuilder<PerformResult> Perform(String Name)
        {
            return Rules.AddRule<PerformResult>(Name);
        }
		
        public RuleBuilder<RT> Value<RT>(String Name)
        {
            return Rules.AddRule<RT>(Name);
        }

		public RuleBuilder<CheckResult> Check(String Name)
        {
            return Rules.AddRule<CheckResult>(Name);
        }

		public void DeclarePerformRuleBook<T0>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0)).Description = Description;
			if (ArgumentNames.Length == 1)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Perform, typeof(T0).Name + " " + ArgumentNames[0]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
		
		public void DeclareValueRuleBook<T0, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0)).Description = Description;
			if (ArgumentNames.Length == 1)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Value, typeof(T0).Name + " " + ArgumentNames[0],  typeof(RT).Name);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}

		public void DeclareCheckRuleBook<T0>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0)).Description = Description;
			if (ArgumentNames.Length == 1)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Check, typeof(T0).Name + " " + ArgumentNames[0]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
				
        public RuleBuilder<T0, PerformResult> Perform<T0>(String Name)
        {
            return Rules.AddRule<T0, PerformResult>(Name);
        }
		
        public RuleBuilder<T0, RT> Value<T0, RT>(String Name)
        {
            return Rules.AddRule<T0, RT>(Name);
        }

		public RuleBuilder<T0, CheckResult> Check<T0>(String Name)
        {
            return Rules.AddRule<T0, CheckResult>(Name);
        }

		public void DeclarePerformRuleBook<T0, T1>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1)).Description = Description;
			if (ArgumentNames.Length == 2)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Perform, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
		
		public void DeclareValueRuleBook<T0, T1, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1)).Description = Description;
			if (ArgumentNames.Length == 2)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Value, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1],  typeof(RT).Name);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}

		public void DeclareCheckRuleBook<T0, T1>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1)).Description = Description;
			if (ArgumentNames.Length == 2)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Check, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
				
        public RuleBuilder<T0, T1, PerformResult> Perform<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, PerformResult>(Name);
        }
		
        public RuleBuilder<T0, T1, RT> Value<T0, T1, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, RT>(Name);
        }

		public RuleBuilder<T0, T1, CheckResult> Check<T0, T1>(String Name)
        {
            return Rules.AddRule<T0, T1, CheckResult>(Name);
        }

		public void DeclarePerformRuleBook<T0, T1, T2>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
			if (ArgumentNames.Length == 3)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Perform, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
		
		public void DeclareValueRuleBook<T0, T1, T2, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
			if (ArgumentNames.Length == 3)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Value, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2],  typeof(RT).Name + " value");
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}

		public void DeclareCheckRuleBook<T0, T1, T2>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1), typeof(T2)).Description = Description;
			if (ArgumentNames.Length == 3)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Check, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
				
        public RuleBuilder<T0, T1, T2, PerformResult> Perform<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, PerformResult>(Name);
        }
		
        public RuleBuilder<T0, T1, T2, RT> Value<T0, T1, T2, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, RT>(Name);
        }

		public RuleBuilder<T0, T1, T2, CheckResult> Check<T0, T1, T2>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, CheckResult>(Name);
        }

		public void DeclarePerformRuleBook<T0, T1, T2, T3>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<PerformResult>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
			if (ArgumentNames.Length == 4)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Perform, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2], typeof(T3).Name + " " + ArgumentNames[3]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
		
		public void DeclareValueRuleBook<T0, T1, T2, T3, RT>(String Name, String Description, params String[] ArgumentNames)
        {
            Rules.FindOrCreateRuleBook<RT>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
			if (ArgumentNames.Length == 4)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Value, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2], typeof(T3).Name + " " + ArgumentNames[3],  typeof(RT).Name + " value");
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}

		public void DeclareCheckRuleBook<T0, T1, T2, T3>(String Name, String Description, params String[] ArgumentNames)
		{
			Rules.FindOrCreateRuleBook<CheckResult>(Name, typeof(T0), typeof(T1), typeof(T2), typeof(T3)).Description = Description;
			if (ArgumentNames.Length == 4)
				MudObjectTransformer.StandardRules.AddRule(Name, MudObjectTransformer.RuleBookType.Check, typeof(T0).Name + " " + ArgumentNames[0], typeof(T1).Name + " " + ArgumentNames[1], typeof(T2).Name + " " + ArgumentNames[2], typeof(T3).Name + " " + ArgumentNames[3]);
			else if (ArgumentNames.Length != 0) throw new InvalidOperationException();
		}
				
        public RuleBuilder<T0, T1, T2, T3, PerformResult> Perform<T0, T1, T2, T3>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, PerformResult>(Name);
        }
		
        public RuleBuilder<T0, T1, T2, T3, RT> Value<T0, T1, T2, T3, RT>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, RT>(Name);
        }

		public RuleBuilder<T0, T1, T2, T3, CheckResult> Check<T0, T1, T2, T3>(String Name)
        {
            return Rules.AddRule<T0, T1, T2, T3, CheckResult>(Name);
        }

	}
}
