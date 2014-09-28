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
                    new FailIfNoMatches(
					    new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.\r\n"),
					new KeyWord("TO", true),
                    new FailIfNoMatches(
					    new Path("DESTINATION"),
                        "You have to specify where to move it to.\r\n")),
				new MoveProcessor(),
				"Teleport an object to a new location. Bypasses take rules.");
		}
	}

	internal class MoveProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["OBJECT"] as Thing;
            var destination = Match.Arguments["DESTINATION"].ToString();
			var room = Mud.GetObject(destination);
            if (room != null)
            {
                Mud.MarkLocaleForUpdate(target);
                Thing.Move(target, room);
                Mud.MarkLocaleForUpdate(room);
            }
            else
            {
                Mud.SendMessage(Actor, "I couldn't find the destination for that move.\r\n");
            }
		}
	}
}
