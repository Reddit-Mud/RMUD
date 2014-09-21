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
			if (String.IsNullOrEmpty(Command)) return;

			if (Command[0] == '@')
			{
				var tokens = Command.Split(' ');
				if (tokens.Length == 0) return;

				if (tokens[0].ToUpper() == "@MATCH")
				{
					var matches = Parser.FindAllGoodMatches(Command.Substring(7), Client.Player);
					Client.Send("Matches: " + matches.Count + "\r\n");
					foreach (var match in matches)
					{
						var builder = new StringBuilder();
						builder.Append(match.Command.Processor.GetType().Name);
						builder.Append(" - ");

						foreach (var arg in match.Match.Arguments)
						{
							builder.Append("[");
							builder.Append(arg.Key);
							builder.Append(" : ");
							builder.Append(arg.Value.ToString());
							builder.Append("] ");
						}

						builder.Append("\r\n");

						Client.Send(builder.ToString());
					}
				}
				else
				{
					Client.Send("I don't recognize that debugging command.");
				}

				return;
			}

            try
			{
				var matchedCommand = Parser.ParseCommand(Command, Client.Player);
				if (matchedCommand != null)
					matchedCommand.Command.Processor.Perform(matchedCommand.Match, Client.Player);
				else
					Client.Send("huh?\r\n");
			}
			catch (Exception e)
			{
				Mud.ClearPendingMessages();
				Client.Send(e.Message);
			}
		}
	}
}
