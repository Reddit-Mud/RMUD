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
                        new KeyWord("EXAMINE", false),
                        new KeyWord("X", false)),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.\r\n")),
                new ExamineProcessor(),
                "Look closely at an object.");

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
					Mud.SendMessage(Actor, MessageScope.Single, "That object is indescribeable.\r\n");
				else
					Mud.SendMessage(Actor, MessageScope.Single, target.Long.Expand(Actor, target as MudObject) + "\r\n");
			}
		}
	}
}
