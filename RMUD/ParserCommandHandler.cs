using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class ParserCommandHandler : IClientCommandHandler
	{
		internal CommandParser Parser;

		public ParserCommandHandler()
		{
			Parser = new CommandParser();

			//Iterate over all types, find ICommandFactories, Create commands
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsSubclassOf(typeof(CommandFactory)))
				{
					var instance = Activator.CreateInstance(type) as CommandFactory;
					instance.Create(Parser);
				}
			}
		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, Client.Player);
				if (matchedCommand != null)
					matchedCommand.Command.Processor.Perform(matchedCommand.Match, Client.Player);
				else
					MudCore.SendMessage(Client, "huh?", true);
			}
			catch (Exception e)
			{
				MudCore.SendMessage(Client, e.Message, true);
			}
		}
	}
}
