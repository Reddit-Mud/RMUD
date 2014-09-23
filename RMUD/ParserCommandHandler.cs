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
					var matches = Parser.ParseCommand(Command.Substring(7), Client.Player);
                    Client.Send("Matched processor: " + matches.Command.Processor.GetType().Name + "\r\n");
					Client.Send("Matches: " + matches.Matches.Count + "\r\n");
                    foreach (var match in matches.Matches)
                    {
                        var builder = new StringBuilder();

                        foreach (var arg in match.Arguments)
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
				else if (tokens[0].ToUpper() == "@TIME")
				{
					var startTime = DateTime.Now;
					var match = Parser.ParseCommand(Command.Substring(6), Client.Player);
					var endTime = DateTime.Now;

					var elapsedTime = endTime - startTime;

					Client.Send(String.Format("Command matching took {0:n0} milliseconds.\r\n", elapsedTime.TotalMilliseconds));
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
					matchedCommand.Command.Processor.Perform(matchedCommand.Matches[0], Client.Player);
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
