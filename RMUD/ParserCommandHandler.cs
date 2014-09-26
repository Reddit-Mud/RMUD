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
                #region Handle debug command

                var tokens = Command.Split(' ');
                if (tokens.Length == 0) return;

                if (tokens[0].ToUpper() == "@MATCH")
                {
                    var startTime = DateTime.Now;
                    var matches = Parser.ParseCommand(Command.Substring(7), Client.Player);
                    var endTime = DateTime.Now;

                    if (matches == null)
                    {
                        Mud.SendMessage(Client, String.Format("Matched nothing in {0:n0} milliseconds.\r\n",
                            (endTime - startTime).TotalMilliseconds));
                    }
                    else
                    {
                        Mud.SendMessage(Client, String.Format("Matched {0} in {1:n0} milliseconds. {2} unique matches.\r\n",
                            matches.Command.Processor.GetType().Name,
                            (endTime - startTime).TotalMilliseconds,
                            matches.Matches.Count));
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

                            Mud.SendMessage(Client, builder.ToString());
                        }
                    }
                }
                else
                {
                    Mud.SendMessage(Client, "I don't recognize that debugging command.");
                }

                #endregion

                return;
            }

            var matchedCommand = Parser.ParseCommand(Command, Client.Player);
            if (matchedCommand != null)
            {
                if (matchedCommand.Matches.Count > 1)
                    Client.CommandHandler = new DisambigCommandHandler(Client, matchedCommand, this);
                else
                    matchedCommand.Command.Processor.Perform(matchedCommand.Matches[0], Client.Player);
            }
            else
                Mud.SendMessage(Client, "huh?\r\n");
        }
    }
}
