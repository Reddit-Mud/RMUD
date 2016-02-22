using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
{
	internal class Quit : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                KeyWord("QUIT"))
                .Manual("Disconnect from the game immediately.")
                .ProceduralRule((match, actor) =>
                {
                    var client = actor.GetPropertyOrDefault<Client>("client");
                    if (client != null)
                    {
                        client.Send("Goodbye...\r\n");
                        client.Disconnect();
                    }

                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
	}
}
