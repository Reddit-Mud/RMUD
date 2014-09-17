using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Take : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("GET", false),
						new KeyWord("TAKE", false)),
					new ObjectMatcher("TARGET"))
				, new TakeProcessor());
		}
	}

	internal class TakeProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"] as Thing;

			if (Actor.ConnectedClient != null)
				Mud.SendEventMessage(Actor, EventMessageScope.Private, target.Long + "\n");
		}
	}
}
