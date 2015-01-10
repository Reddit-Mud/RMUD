using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace CloakOfDarkness
{
	public class settings : RMUD.Settings
	{
        public static void AtStartup()
        {
            GlobalRules.Perform<Actor>("player joined")
                .Do((actor) =>
                {
                    SendMessage(actor, "Hurrying through the rainswept November night, you're glad to see the bright lights of the Opera House. It's surprising that there aren't more people about but, hey, what do you expect in a cheap demo game...?");
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
            NewPlayerStartRoom = "Foyer";
            PlayerBaseObject = "Player";
        }
	}
}
