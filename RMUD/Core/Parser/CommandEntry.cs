using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class StandardProceduralRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("before acting", "[Match, Actor] : Considered before performing in world actions.");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("after acting", "[Match, Actor] : Considered after performing in world actions.");
        }
    }

    public sealed class CommandEntry : ManPage
    {
        internal CommandTokenMatcher Matcher;
        internal String ManualName = "";
        internal String ManualPage = "";
        internal StringBuilder GeneratedManual = null;
        internal PerformRuleBook ProceduralRules;

        public CommandEntry()
        {
            ManPages.Pages.Add(this);
            GeneratedManual = new StringBuilder();
            ProceduralRules = new PerformRuleBook(Core.GlobalRules.Rules)
            {
                ArgumentTypes = new List<Type>(new Type[] { typeof(PossibleMatch), typeof(Actor) }),
            };
        }

        public bool IsNamed(String Name)
        {
            return ManualName == Name;
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

        public CommandEntry Check(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the check rulebook '" + RuleName + " with arguments " + String.Join(", ", RuleArguments));

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    if (Core.GlobalRules.ConsiderCheckRule(RuleName, RuleArguments.Select(a => match.ValueOrDefault(a)).ToArray()) == CheckResult.Allow)
                        return PerformResult.Continue;
                    return PerformResult.Stop;
                }),
                DescriptiveName = "Procedural rule to check " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry Perform(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the perform rulebook '" + RuleName + " with arguments " + String.Join(", ", RuleArguments) + " and discard the result.");

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    Core.GlobalRules.ConsiderPerformRule(RuleName, RuleArguments.Select(a => match.ValueOrDefault(a)).ToArray());
                    return PerformResult.Continue;
                }),
                DescriptiveName = "Procedural rule to perform " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry AbideBy(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the perform rulebook '" + RuleName + " with arguments " + String.Join(", ", RuleArguments) + " and abide by the result.");

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                    Core.GlobalRules.ConsiderPerformRule(RuleName, RuleArguments.Select(a => match.ValueOrDefault(a)).ToArray())
                    ),
                DescriptiveName = "Procedural rule to abide by " + RuleName
            };
            ProceduralRules.AddRule(rule);
            return this;
        }

        public CommandEntry BeforeActing()
        {
            ProceduralRules.AddRule(new Rule<PerformResult>{
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>((match, actor) => Core.GlobalRules.ConsiderMatchBasedPerformRule("before acting", match, actor)),
                DescriptiveName = "Before acting procedural rule."});
            return this;
        }

        public CommandEntry AfterActing()
        {
            ProceduralRules.AddRule(new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>((match, actor) => 
                {
                    Core.GlobalRules.ConsiderMatchBasedPerformRule("after acting", match, actor);
                    return PerformResult.Continue;
                }),
                DescriptiveName = "After acting procedural rule."
            });
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
                    MudObject.MarkLocaleForUpdate(match["ACTOR"] as MudObject);
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
            MudObject.SendMessage(To, builder.ToString());
        }
    }
}
