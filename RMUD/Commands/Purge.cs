using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Purge : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("PURGE", false)),
                new PurgeProcessor(),
                "Purge the dynamic database from disc.");
        }
	}

	internal class PurgeProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;
            var tempCommand = new CommandParser.MatchedCommand(new CommandParser.CommandEntry
            {
                Processor = new SecondStagePurgeProcessor(),
            }, new PossibleMatch[] { Match });

            Actor.ConnectedClient.CommandHandler = new ConfirmCommandHandler(Actor.ConnectedClient, tempCommand, Actor.ConnectedClient.CommandHandler);
		}
	}

    internal class SecondStagePurgeProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            Mud.CommandTimeoutEnabled = false;

            if (System.IO.Directory.Exists(Mud.DynamicPath))
                System.IO.Directory.Delete(Mud.DynamicPath, true);

            Mud.SendMessage(Actor, "Dynamic data has been purged.");
        }
    }
}
