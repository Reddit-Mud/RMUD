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
					a.ConnectedClient.CommandHandler = Mud.ParserCommandHandler;
					MudObject.Move(a, Mud.LoadObject("dummy"));
					Mud.EnqueuClientCommand(a.ConnectedClient, "look");
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
					Client.Send("I do not understand.");
			}
			catch (Exception e)
			{
				Mud.ClearPendingMessages();
				Client.Send(e.Message);
			}
		}
	}
}
