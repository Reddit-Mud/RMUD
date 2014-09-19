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
					new ObjectMatcher("TARGET", new InScopeObjectSource()),
					new KeyWord("TO", true),
					new Path("DESTINATION"))
				, new MoveProcessor(),
				"Teleport an object to a new location. Bypasses take rules.");
		}
	}

	internal class MoveProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as Thing;
			var destination = Match.Arguments["DESTINATION"].ToString();
			var room = Mud.GetObject(destination);
			if (room != null)
				Thing.Move(target, room);
		}
	}
}
