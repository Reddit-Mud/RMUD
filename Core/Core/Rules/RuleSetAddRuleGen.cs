//This is generated code. Do not modify this file; modify the template that produces it.

using System;

namespace RMUD
{
	public partial class RuleSet
	{	
		public RuleBuilder<RT> AddRule<RT>(String Name)
		{
			var rule = new Rule<RT>();
			FindOrCreateRuleBook<RT>(Name, 0).AddRule(rule);
			return new RuleBuilder<RT> { Rule = rule };
		}

		public RuleBuilder<T0, RT> AddRule<T0, RT>(String Name)
		{
			var rule = new Rule<RT>();
			FindOrCreateRuleBook<RT>(Name, 1).AddRule(rule);
			return new RuleBuilder<T0, RT> { Rule = rule };
		}

		public RuleBuilder<T0, T1, RT> AddRule<T0, T1, RT>(String Name)
		{
			var rule = new Rule<RT>();
			FindOrCreateRuleBook<RT>(Name, 2).AddRule(rule);
			return new RuleBuilder<T0, T1, RT> { Rule = rule };
		}

		public RuleBuilder<T0, T1, T2, RT> AddRule<T0, T1, T2, RT>(String Name)
		{
			var rule = new Rule<RT>();
			FindOrCreateRuleBook<RT>(Name, 3).AddRule(rule);
			return new RuleBuilder<T0, T1, T2, RT> { Rule = rule };
		}

		public RuleBuilder<T0, T1, T2, T3, RT> AddRule<T0, T1, T2, T3, RT>(String Name)
		{
			var rule = new Rule<RT>();
			FindOrCreateRuleBook<RT>(Name, 4).AddRule(rule);
			return new RuleBuilder<T0, T1, T2, T3, RT> { Rule = rule };
		}

	}
}
