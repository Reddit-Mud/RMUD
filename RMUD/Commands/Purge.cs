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

		}
	}

}
