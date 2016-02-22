using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using SharpRuleEngine;

namespace RMUD
{
    public static partial class Core
    {
        /// <summary>
        /// The command that is currently executing. Rules can use this property to figure out what command they have
        /// been invoked by.
        /// </summary>
        public static PossibleMatch ExecutingCommand { get; private set; }

        /// <summary>
        /// Actually carryout the command, following all of it's rules, including the before and after command rules.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Match"></param>
        /// <param name="Actor"></param>
        /// <returns>The result of the command's procedural rules.</returns>
        private static PerformResult ExecuteCommand(CommandEntry Command, PossibleMatch Match, MudObject Actor)
        {
            var result = PerformResult.Stop;
            Match.Upsert("COMMAND", Command);
            if (GlobalRules.ConsiderMatchBasedPerformRule("before command", Match, Actor) == PerformResult.Continue)
            {
                result = Command.ProceduralRules.Consider(Match, Actor);
                GlobalRules.ConsiderMatchBasedPerformRule("after command", Match, Actor);
            }
            GlobalRules.ConsiderPerformRule("after every command", Actor);
            return result;
        }

        public static void ProcessPlayerCommand(CommandEntry Command, PossibleMatch Match, MudObject Actor)
        {
            ExecutingCommand = Match;
            try
            {
                ExecuteCommand(Command, Match, Actor);
            }
            finally
            {
                ExecutingCommand = null;
            }
        }

        /// <summary>
        /// Try to execute a command immediately. This does not bypass the before and after command rules. This is intended
        /// to be used by rules to implement implicit actions. For example, the 'go' command will attempt to open closed
        /// doors by calling
        ///     Core.Try("StandardActions:Open", Core.ExecutingCommand.With("SUBJECT", link), actor);
        /// </summary>
        /// <param name="CommandID">The ID of the command to try, assigned when the command is created.</param>
        /// <param name="Match"></param>
        /// <param name="Actor"></param>
        /// <returns>The result of the command's proceedural rules.</returns>
        public static PerformResult Try(String CommandID, PossibleMatch Match, MudObject Actor)
        {
            var parentCommand = ExecutingCommand;
            try
            {
                var command = Core.DefaultParser.FindCommandWithID(CommandID);
                if (command == null) return PerformResult.Stop;
                return ExecuteCommand(command, Match, Actor);
            }
            finally
            {
                ExecutingCommand = parentCommand;
            }
        }
    }

    public class BeforeAndAfterCommandRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, MudObject>("before command", "[Match, Actor] : Considered before every command's procedural rules are run.", "match", "actor");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, MudObject>("after command", "[Match, Actor] : Considered after every command's procedural rules are run, unless the before command rules stopped the command.", "match", "actor");

            GlobalRules.DeclarePerformRuleBook<MudObject>("after every command", "[Actor] : Considered after every command, even if earlier rules stopped the command.", "actor");
        }
    }
}
