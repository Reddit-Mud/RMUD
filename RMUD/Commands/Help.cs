using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Help : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Or(
					new KeyWord("HELP", false),
					new KeyWord("?", false)),
				new HelpProcessor(),
				"Display a list of all defined commands.");
		}
	}

	internal class HelpProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			if (Actor.ConnectedClient != null)
			{
				var builder = new StringBuilder();
				foreach (var command in Mud.ParserCommandHandler.Parser.Commands)
				{
					builder.Append(command.Matcher.Emit());
					builder.Append(" -- ");
					builder.Append(command.HelpText);
					builder.Append("\r\n");
				}

				Mud.SendMessage(Actor, MessageScope.Single, builder.ToString());
			}
		}
	}
}
