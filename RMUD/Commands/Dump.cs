using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Dump : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("DUMP", false),
                    new FailIfNoMatches(
                        new Path("TARGET"),
                        "It helps if you give me a path.")),
                new DumpProcessor(),
                "Dump a database source file.");
        }
	}

	internal class DumpProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"].ToString();
            var source = Mud.LoadRawLocalSourceFile(target);

            Mud.SendMessage(Actor, "[" + target + "]\r\n" + source);
		}
	}

}
