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
               new KeyWord("QUIT", false),
                new QuitProcessor(),
                "Disconnect immediately");
        }
	}

	internal class QuitProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            Client client = null;
            if (Actor != null && Actor.ConnectedClient != null)
            {
                client = Actor.ConnectedClient;
            }
            else
            {
                client = Match.Arguments["CLIENT"] as Client;
            }

            if (client != null)
            {
                client.Send("Goodbye...\r\n");
                client.Disconnect();
            }
        }
	}
}
