using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Akkoteaque
{
	public class settings : RMUD.Settings
	{
        public static void AtStartup(RuleEngine GlobalRules)
        {
            
            GlobalRules.Perform<Actor>("player joined")
                .Do((actor) =>
                {
                    return PerformResult.Continue;
                });

            GlobalRules.Perform<PossibleMatch, Actor>("before command")
                .First
                .Do((match, actor) =>
                    {
                        Console.WriteLine();
                        return PerformResult.Continue;
                    });

            GlobalRules.Perform<Actor>("after every command")
                .Last
                .Do((actor) =>
                {
                    Console.WriteLine();
                    return PerformResult.Continue;
                });
        }

        public settings()
        {
            NewPlayerStartRoom = "Jetty";
            PlayerBaseObject = "Player";
        }
	}
}
