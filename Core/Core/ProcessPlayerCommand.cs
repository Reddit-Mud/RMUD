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
        public static PossibleMatch ExecutingCommand { get; private set; }

        private static PerformResult ExecuteCommand(CommandEntry Command, PossibleMatch Match, Actor Actor)
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

        public static void ProcessPlayerCommand(CommandEntry Command, PossibleMatch Match, Actor Actor)
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

        public static PerformResult Try(String CommandID, PossibleMatch Match, Actor Actor)
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
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("before command", "[Match, Actor] : Considered before every command's procedural rules are run.", "match", "actor");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("after command", "[Match, Actor] : Considered after every command's procedural rules are run, unless the before command rules stopped the command.", "match", "actor");

            GlobalRules.DeclarePerformRuleBook<Actor>("after every command", "[Actor] : Considered after every command, even if earlier rules stopped the command.", "actor");

            GlobalRules.DeclarePerformRuleBook<Actor>("player joined", "[Player] : Considered when a player enters the game.", "actor");

            GlobalRules.DeclarePerformRuleBook<Actor>("player left", "[Player] : Considered when a player leaves the game.", "actor");

            GlobalRules.Perform<Actor>("player joined")
                .First
                .Do((actor) =>
                {
                    MudObject.Move(actor, MudObject.GetObject(Core.SettingsObject.NewPlayerStartRoom));
                    return PerformResult.Continue;
                })
                .Name("Move to start room rule.");

        }
    }
}
