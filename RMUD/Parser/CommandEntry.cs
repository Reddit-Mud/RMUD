using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class CommandEntry : ManPage
    {
        internal CommandTokenMatcher Matcher;
        internal String ManualName = "";
        internal String ManualPage = "";
        internal StringBuilder GeneratedManual = null;
        internal ActionRuleBook ProceduralRules;

        public void VerifyCompleteness()
        {
            if (String.IsNullOrEmpty(ManualName))
                Mud.LogWarning("Command does not have name set - " + Matcher.Emit());
            if (String.IsNullOrEmpty(ManualPage))
                Mud.LogWarning("No manual for command " + ManualName);
        }

        public CommandEntry()
        {
            Mud.ManPages.Add(this);
            GeneratedManual = new StringBuilder();
            ProceduralRules = new ActionRuleBook
            {
                ArgumentTypes = new List<Type>(new Type[] { typeof(PossibleMatch), typeof(Actor) }),
            };
        }

        public CommandEntry Name(String Name)
        {
            this.ManualName = Name.ToUpper();
            return this;
        }

        public CommandEntry Manual(String Manual)
        {
            this.ManualPage = Manual;
            return this;
        }

        public CommandEntry ProceduralRule(Func<PossibleMatch, Actor, PerformResult> Rule, String Name = "an unamed proceddural rule")
        {
            GeneratedManual.AppendLine("Consider " + Name);

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
            GeneratedManual.AppendLine("Consider the check rulebook '" + RuleName + "' on " + Target + " with arguments " + String.Join(", ", RuleArguments));

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    var ruleTarget = match[Target];
                    if (GlobalRules.ConsiderCheckRule(RuleName, ruleTarget as MudObject, RuleArguments.Select(a => match[a]).ToArray()) == CheckResult.Allow)
                        return PerformResult.Continue;
                    return PerformResult.Stop;
                }),
                DescriptiveName = "Procedural rule to check " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry Value<T>(String RuleName, T ExpectedValue, String Target, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Assert that the value rulebook '" + RuleName + "' on " + Target + " with arguments " + String.Join(", ", RuleArguments) + " results in " + ExpectedValue.ToString());

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    var ruleTarget = match[Target];
                    var ruleResult = GlobalRules.ConsiderValueRule<T>(RuleName, ruleTarget as MudObject, RuleArguments.Select(a => match[a]).ToArray());
                    if (EqualityComparer<T>.Default.Equals(ruleResult, ExpectedValue))
                        return PerformResult.Continue;
                    return PerformResult.Stop;
                }),
                DescriptiveName = "Procedural rule to assert on " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry Perform(String RuleName, String Target, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the perform rulebook '" + RuleName + "' on " + Target + " with arguments " + String.Join(", ", RuleArguments));

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    var ruleTarget = match[Target];
                    GlobalRules.ConsiderPerformRule(RuleName, ruleTarget as MudObject, RuleArguments.Select(a => match[a]).ToArray());
                    return PerformResult.Continue;
                }),
                DescriptiveName = "Procedural rule to perform " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry MarkLocaleForUpdate()
        {
            GeneratedManual.AppendLine("Consider the mark locale for update rule");

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    Mud.MarkLocaleForUpdate(match["ACTOR"] as MudObject);
                    return PerformResult.Continue;
                }),
                DescriptiveName = "Procedural rule to mark locale for update."
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        string ManPage.Name
        {
            get { return ManualName; }
        }

        void ManPage.SendManPage(MudObject To)
        {
            var builder = new StringBuilder();
            builder.AppendLine(ManualName);
            builder.AppendLine(Matcher.Emit());
            builder.AppendLine();
            if (GeneratedManual != null) builder.AppendLine(GeneratedManual.ToString());
            builder.Append(ManualPage);
            Mud.SendMessage(To, builder.ToString());
        }
    }
}
