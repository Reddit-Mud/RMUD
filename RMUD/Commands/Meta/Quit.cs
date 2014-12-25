using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Quit : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                KeyWord("QUIT"),
                "Disconnect immediately")
                .Manual("Disconnect from the game immediately.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor != null && actor.ConnectedClient != null)
                        match.Arguments.Upsert("CLIENT", actor.ConnectedClient);
                    if (match.Arguments.ContainsKey("CLIENT"))
                    {
                        (match.Arguments["CLIENT"] as Client).Send("Goodbye...\r\n");
                        (match.Arguments["CLIENT"] as Client).Disconnect();
                    }
                    return PerformResult.Continue;
                });
        }
	}
}
