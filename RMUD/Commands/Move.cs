using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Move : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new RankGate(500),
					new KeyWord("MOVE", false),
					new ObjectMatcher("TARGET"),
					new KeyWord("TO", true),
					new Path("DESTINATION"))
				, new MoveProcessor());
		}
	}

	internal class MoveProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as MudObject;
			var destination = Match.Arguments["DESTINATION"].ToString();
			var room = Mud.GetObject(destination);
			if (room != null)
				MudObject.Move(target, room);
		}
	}
}
