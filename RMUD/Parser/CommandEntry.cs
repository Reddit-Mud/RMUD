using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class CommandEntry
    {
        internal CommandTokenMatcher Matcher;
        internal CommandProcessor Processor;
        internal String HelpText;
        internal ActionRuleBook ProceduralRules;

        private void PrepareProceduralRuleBook()
        {
            if (ProceduralRules == null)
                ProceduralRules = new ActionRuleBook
                {
                    ArgumentTypes = new List<Type>(new Type[] { typeof(PossibleMatch), typeof(Actor) }),
                };
        }

        public CommandEntry ProceduralRule(Func<PossibleMatch, Actor, PerformResult> Rule, String Name = "")
        {
            PrepareProceduralRuleBook();
            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper(Rule),
                DescriptiveName = Name
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry Check(String RuleName, String Target, params String[] RuleArguments)
        {
            PrepareProceduralRuleBook();
            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    var ruleTarget = match.Arguments[Target];
                    if (GlobalRules.ConsiderCheckRule(RuleName, ruleTarget as MudObject, RuleArguments.Select(a => match.Arguments[a]).ToArray()) == CheckResult.Allow)
                        return PerformResult.Continue;
                    return PerformResult.Stop;
                }),
                DescriptiveName = "Procedural rule to check " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry Perform(String RuleName, String Target, params String[] RuleArguments)
        {
            PrepareProceduralRuleBook();
            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    var ruleTarget = match.Arguments[Target];
                    GlobalRules.ConsiderPerformRule(RuleName, ruleTarget as MudObject, RuleArguments.Select(a => match.Arguments[a]).ToArray());
                    return PerformResult.Continue;
                }),
                DescriptiveName = "Procedural rule to perform " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry MarkLocaleForUpdate()
        {
            PrepareProceduralRuleBook();
            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    Mud.MarkLocaleForUpdate(match.Arguments["ACTOR"] as MudObject);
                    return PerformResult.Continue;
                }),
                DescriptiveName = "Procedural rule to mark locale for update."
            };
            ProceduralRules.AddRule(rule);
            return this;
        }
    }
}
