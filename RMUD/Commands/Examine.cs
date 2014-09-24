using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Examine : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("LOOK", false),
						new KeyWord("EXAMINE", false),
						new KeyWord("X", false)),
					new KeyWord("AT", true),
					new ObjectMatcher("OBJECT", new InScopeObjectSource())),
				new ExamineProcessor(),
				"Look closely at an object.");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("LOOK", false),
						new KeyWord("EXAMINE", false),
						new KeyWord("X", false)),
					new KeyWord("AT", true),
					new Rest("ERROR"))
				, new ReportError("I don't see that here.\r\n"),
				"Error reporting command");
		}
	}

	internal class ExamineProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["OBJECT"] as IDescribed;

			if (Actor.ConnectedClient != null)
			{
				if (target == null)
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "That object is indescribeable.\r\n");
				else
					Mud.SendEventMessage(Actor, EventMessageScope.Single, target.Long.Expand(Actor, target as MudObject) + "\r\n");
			}
		}
	}
}
