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
					new ObjectMatcher("TARGET"))
				, new ExamineProcessor());
		}
	}

	internal class ExamineProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as Thing;
			
			if (Actor.ConnectedClient != null)
			{
				MudCore.SendMessage(Actor.ConnectedClient, target.Long + "\n", false);
			}
		}
	}
}
