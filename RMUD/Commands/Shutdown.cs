using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Shutdown : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("@SHUTDOWN", false)),
                new ShutdownProcessor(),
                "Shutdown the server.");
        }
	}

	internal class ShutdownProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            Mud.SendMessage(Actor, "Not implemented. :(");
        }
	}

}
