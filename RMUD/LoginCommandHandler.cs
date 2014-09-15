using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class LoginCommandHandler : IClientCommandHandler
	{
		internal CommandParser Parser;

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

			Parser.AddCommand(
				new Sequence(new KeyWord("LOGIN", false), new SingleWord("NAME")),
				new CommandProcessorWrapper((m, a) =>
				{
					a.Short = m.Arguments["NAME"].ToString();
					a.ConnectedClient.CommandHandler = MudCore.ParserCommandHandler;
					a.Location = "dummy";
				}));
		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, Client.Player);
				if (matchedCommand != null)
					matchedCommand.Command.Processor.Perform(matchedCommand.Match, Client.Player);
				else
					MudCore.SendImmediateMessage(Client, "I do not understand.");
			}
			catch (Exception e)
			{
				MudCore.SendImmediateMessage(Client, e.Message);
			}
		}
	}
}
