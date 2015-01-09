using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Core
    {
        public static void ProcessPlayerCommand(CommandEntry Command, PossibleMatch Match, Actor Actor)
        {
            Match.Upsert("COMMAND", Command);
            if (GlobalRules.ConsiderMatchBasedPerformRule("before command", Match, Actor) == PerformResult.Continue)
            {
                Command.ProceduralRules.Consider(Match, Actor);
                GlobalRules.ConsiderMatchBasedPerformRule("after command", Match, Actor);
            }
            GlobalRules.ConsiderPerformRule("after every command", Actor);
        }
    }

    public class BeforeAndAfterCommandRules 
    {
        public static void AtStartup()
        {
            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("before command", "[Match, Actor] : Considered before every command's procedural rules are run.");

            GlobalRules.DeclarePerformRuleBook<PossibleMatch, Actor>("after command", "[Match, Actor] : Considered after every command's procedural rules are run, unless the before command rules stopped the command.");

            GlobalRules.DeclarePerformRuleBook<Actor>("after every command", "[Actor] : Considered after every command, even if earlier rules stopped the command.");

            GlobalRules.DeclarePerformRuleBook<Actor>("player joined", "[Player] : Considered when a player enters the game.");

            GlobalRules.DeclarePerformRuleBook<Actor>("player left", "[Player] : Considered when a player leaves the game.");
        }
    }
}
