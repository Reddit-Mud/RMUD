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
        public CheckRuleBook CheckRules;

        private void PrepareCheckRuleBook()
        {
            if (CheckRules == null)
                CheckRules = new CheckRuleBook
                {
                    ArgumentTypes = new List<Type>(new Type[] {
                        typeof(PossibleMatch),
                        typeof(Actor)
                    })
                };
        }

        public CommandEntry CheckRule(Func<PossibleMatch, Actor, CheckResult> Rule)
        {
            PrepareCheckRuleBook();

            return this;
        }

        public CommandEntry MustBeVisible(String ObjectName)
        {
            PrepareCheckRuleBook();

            var builder = new RuleBuilder<PossibleMatch, Actor, CheckResult> { Rule = new Rule<CheckResult>() }.Do((pm, actor) =>
            {
                if (!pm.Arguments.ContainsKey(ObjectName))
                {
                    Mud.SendMessage(actor, "There was an error in the implementation of that command.");
                    return CheckResult.Disallow;
                }

                var obj = pm.Arguments[ObjectName] as MudObject;
                if (!Mud.IsVisibleTo(actor, obj))
                {
                    Mud.SendMessage(actor, "That doesn't seem to be here anymore.");
                    return CheckResult.Disallow;
                }

                return CheckResult.Continue;
            });
            CheckRules.AddRule(builder.Rule);
            return this;
        }
    }
}
