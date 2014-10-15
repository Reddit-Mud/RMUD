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
            if (Actor.ConnectedClient != null)
                Actor.ConnectedClient.Disconnect();
        }        
	}
}
