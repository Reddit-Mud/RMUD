using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Sleep : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new RankGate(500),
					new KeyWord("SLEEP", false),
                    new Number("MILLISECONDS")),
				new SleepProcessor(),
				"Hang the game for everyone.");
		}
	}

    internal class SleepProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            System.Threading.Thread.Sleep((Match.Arguments["MILLISECONDS"] as int?).Value);

            Mud.SendMessage(Actor, "SLEPT!");
        }
    }
}
