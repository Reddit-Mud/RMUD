using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

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

    /// <summary>
    /// Represents a single possible command. CommandEntry provides a fluent interface for creating commands.
    /// Most methods return the instance they were invoked on to allow methods to be chained.
    /// </summary>
    public sealed class CommandEntry : ManPage
    {
        internal CommandTokenMatcher Matcher;
        public String ManualName { get; internal set; }
        internal String ManualPage = "";
        internal StringBuilder GeneratedManual = null;
        internal PerformRuleBook ProceduralRules;
        internal String _ID = "";
        public String SourceModule { get; internal set; }
        
        public CommandEntry()
        {
            ManualName = "";
            SourceModule = null;

            ManPages.Pages.Add(this);
            GeneratedManual = new StringBuilder();
            ProceduralRules = new PerformRuleBook(Core.GlobalRules.Rules)
            {
                ArgumentCount = 2
                /*ArgumentTypes = new List<Type>(new Type[] { typeof(PossibleMatch), typeof(Actor) }),*/
            };
        }

        /// <summary>
        /// Set this command's ID string.
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns>This command</returns>
        public CommandEntry ID(String _ID)
        {
            this._ID = _ID;
            return this;
        }

        public String GetID()
        {
            return this._ID;
        }

        public bool IsNamed(String Name)
        {
            return ManualName == Name;
        }

        /// <summary>
        /// Set this command's Name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>This command</returns>
        public CommandEntry Name(String Name)
        {
            this.ManualName = Name.ToUpper();
            return this;
        }

        /// <summary>
        /// Set manual text for this command.
        /// </summary>
        /// <param name="Manual"></param>
        /// <returns>This command</returns>
        public CommandEntry Manual(String Manual)
        {
            this.ManualPage = Manual;
            return this;
        }

        /// <summary>
        /// Add a procedural rule to this command.
        /// </summary>
        /// <param name="Rule"></param>
        /// <param name="Name"></param>
        /// <returns>This command</returns>
        public CommandEntry ProceduralRule(Func<PossibleMatch, Actor, PerformResult> Rule, String Name = "an unamed procedural rule")
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

        /// <summary>
        /// Add a new procedural rule to this command that invokes a check rule. If the checkrule fails, 
        /// processing of the proceedural rules is stopped.
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="RuleArguments"></param>
        /// <returns>This command</returns>
        public CommandEntry Check(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the check rulebook '" + RuleName + "' with arguments " + String.Join(", ", RuleArguments));

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

        /// <summary>
        /// Add a proceedural rule to this command that considers the specified perform rule.
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="RuleArguments"></param>
        /// <returns>This command</returns>
        public CommandEntry Perform(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the perform rulebook '" + RuleName + "' with arguments " + String.Join(", ", RuleArguments) + " and discard the result.");

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

        /// <summary>
        /// Add a proceedural rule to this command that considers the specified perform rule. Unlike Perform above,
        /// if the perform rule returns stop, the proceedural rules will also be stopped.
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="RuleArguments"></param>
        /// <returns>This command</returns>
        public CommandEntry AbideBy(String RuleName, params String[] RuleArguments)
        {
            GeneratedManual.AppendLine("Consider the perform rulebook '" + RuleName + "' with arguments " + String.Join(", ", RuleArguments) + " and abide by the result.");

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

        /// <summary>
        /// Add a proceedural rule to this command that invokes the before acting rulebook.
        /// </summary>
        /// <returns>This command</returns>
        public CommandEntry BeforeActing()
        {
            GeneratedManual.AppendLine("Consider the before acting rules.");
            ProceduralRules.AddRule(new Rule<PerformResult>{
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>((match, actor) => Core.GlobalRules.ConsiderMatchBasedPerformRule("before acting", match, actor)),
                DescriptiveName = "Before acting procedural rule."});
            return this;
        }

        /// <summary>
        /// Add a proceedural rule to this command that invokes the after acting rulebook.
        /// </summary>
        /// <returns>This command</returns>
        public CommandEntry AfterActing()
        {
            GeneratedManual.AppendLine("Consider the after acting rules.");
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

        /// <summary>
        /// Add a procedural rule to this command that marks the locale of the actor that entered the command
        /// for update.
        /// </summary>
        /// <returns>This command</returns>
        public CommandEntry MarkLocaleForUpdate()
        {
            GeneratedManual.AppendLine("Consider the mark locale for update rule");

            var rule = new Rule<PerformResult>
            {
                BodyClause = RuleDelegateWrapper<PerformResult>.MakeWrapper<PossibleMatch, Actor>(
                (match, actor) =>
                {
                    Core.MarkLocaleForUpdate(match["ACTOR"] as MudObject);
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
            if (!String.IsNullOrEmpty(SourceModule)) builder.AppendFormat("SOURCE MODULE: {0}\n", SourceModule);
            if (!String.IsNullOrEmpty(_ID)) builder.AppendFormat("ID specified: {0}\n", _ID);
            else builder.Append("NO ID SPECIFIED\n");
            builder.AppendLine();
            builder.AppendLine("Rules invoked by command:");
            if (GeneratedManual != null) builder.AppendLine(GeneratedManual.ToString());
            builder.Append(ManualPage);
            MudObject.SendMessage(To, builder.ToString());
        }
    }
}
