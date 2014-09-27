using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Drop : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new KeyWord("DROP", false),
					new ObjectMatcher("SUBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld, "SUBJECTSCORE")),
				new DropProcessor(),
				"Drop something",
                "SUBJECTSCORE");
		}
	}

	internal class DropProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as Thing;
			if (target == null)
			{
				if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "Drop what again?\r\n");
			}
			else
			{
				if (!Actor.Contains(target))
				{
					Mud.SendMessage(Actor, "You aren't holding that.\r\n");
					return;
				}

				var dropRules = target as IDropRules;
				if (dropRules != null && !dropRules.CanDrop(Actor))
				{
					Mud.SendMessage(Actor, "You can't drop that.\r\n");
					return;
				}

				Mud.SendMessage(Actor, MessageScope.Single, "You drop " + target.Indefinite + "\r\n");
				Mud.SendMessage(Actor, MessageScope.External, Actor.Short + " drops " + target.Indefinite + "\r\n");
				Thing.Move(target, Actor.Location);

				if (dropRules != null) dropRules.HandleDrop(Actor);

                Mud.MarkLocaleForUpdate(target);
			}
		}
	}
}
