using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Meta
{
	internal class Cons : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Core.StandardMessage("cons", "Results of consistency check:");
            Core.StandardMessage("cons no results", "I found nothing wrong.");

            Parser.AddCommand(
                Sequence(
                    KeyWord("!CONS"),
                    Optional(KeyWord("LOCAL"), "LOCAL")))
                .ID("Meta:Cons")
                .Manual("Scan all defined commands for ommissions, then scan loaded objects for omissions.")
                .ProceduralRule((match, actor) =>
                {
                    var localScan = false;
                    if (match.ContainsKey("LOCAL"))
                        localScan = (match["LOCAL"] as bool?).Value;

                    var resultsFound = 0;

                    MudObject.SendMessage(actor, "@cons");

                    if (!localScan)
                        foreach (var command in Core.DefaultParser.Commands)
                        {
                            if (String.IsNullOrEmpty(command._ID)) 
                            {
                                resultsFound += 1;
                                MudObject.SendMessage(actor, "Command has no ID set: " + command.ManualName + " from " + command.SourceModule);
                            }
                        }

                    if (resultsFound == 0)
                        MudObject.SendMessage(actor, "@cons no results");


                    return SharpRuleEngine.PerformResult.Continue;
                });
               
		}
	}
}
