using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Version : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
                new Or(
			        new KeyWord("VERSION", false),
                    new KeyWord("VER", false)),
				new VersionProcessor(),
				"See what version the server is running.");
		}
	}

	internal class VersionProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;

            Mud.SendMessage(Actor, "RMUD Veritas III\r\n");
		}
	}
}
