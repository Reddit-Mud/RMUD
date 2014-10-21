using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Save : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("SAVE", false)),
                new PurgeProcessor(),
                "Save game state to disc.");
        }
	}

	internal class SaveProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            Mud.CommandTimeoutEnabled = false;

            Mud.SendGlobalMessage("The database is being saved. There may be a brief delay.\r\n");
            Mud.SendPendingMessages();

            var saved = Mud.SaveActiveInstances();

            Mud.SendGlobalMessage("The database has been saved.\r\n");
            Mud.SendMessage(Actor, String.Format("I saved {0} persistent objects.\r\n", saved));
		}
	}

}
