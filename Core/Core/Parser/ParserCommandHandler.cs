using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class ParserCommandHandler : ClientCommandHandler
	{
		public CommandParser Parser;

		public ParserCommandHandler(CommandParser Parser)
		{
            this.Parser = Parser;
		}

        public void HandleCommand(PendingCommand Command)
        {
            if (String.IsNullOrEmpty(Command.RawCommand)) return;

            bool displayMatches = false;
            bool displayTime = false;

            if (Command.RawCommand[0] == '@')
            {
                if (Command.RawCommand.ToUpper().StartsWith("@MATCH "))
                {
                    Command.RawCommand = Command.RawCommand.Substring("@MATCH ".Length);
                    displayMatches = true;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@TIME "))
                {
                    Command.RawCommand = Command.RawCommand.Substring("@TIME ".Length);
                    displayTime = true;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@DEBUG "))
                {
                    Command.RawCommand = Command.RawCommand.Substring("@DEBUG ".Length);
                    if (Command.Actor.Rank < 500)
                    {
                        MudObject.SendMessage(Command.Actor, "You do not have sufficient rank to use the debug command.");
                        return;
                    }

                    Core.CommandTimeoutEnabled = false;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@RULES "))
                {
                    Command.RawCommand = Command.RawCommand.Substring("@RULES ".Length);
                    Core.GlobalRules.LogRules(Command.Actor);
                }
                else
                {
                    MudObject.SendMessage(Command.Actor, "I don't recognize that debugging command.");
                    return;
                }
            }

            var startTime = DateTime.Now;

            var matchedCommand = Parser.ParseCommand(Command);

            if (displayMatches)
            {
                var matchEndTime = DateTime.Now;

                if (matchedCommand == null)
                {
                    MudObject.SendMessage(Command.Actor, String.Format("Matched nothing in {0:n0} milliseconds.",
                        (matchEndTime - startTime).TotalMilliseconds));
                }
                else
                {
                    MudObject.SendMessage(Command.Actor, String.Format("Matched {0} in {1:n0} milliseconds. {2} unique matches.",
                        matchedCommand.Command.ManualName,
                        (matchEndTime - startTime).TotalMilliseconds,
                        matchedCommand.Matches.Count));
                    foreach (var match in matchedCommand.Matches)
                    {
                        var builder = new StringBuilder();

                        foreach (var arg in match)
                        {
                            builder.Append("[");
                            builder.Append(arg.Key);
                            builder.Append(" : ");
                            builder.Append(arg.Value.ToString());
                            builder.Append("] ");
                        }

                        MudObject.SendMessage(Command.Actor, builder.ToString());
                    }
                }
            }
            else
            {

                if (matchedCommand != null)
                {
                    if (matchedCommand.Matches.Count > 1)
                        Command.Actor.CommandHandler = new DisambigCommandHandler(Command.Actor, matchedCommand, this);
                    else
                        Core.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], Command.Actor);
                }
                else
                    MudObject.SendMessage(Command.Actor, "huh?");
            }

            Core.GlobalRules.LogRules(null);

            if (displayTime)
            {
                var endTime = DateTime.Now;

                MudObject.SendMessage(Command.Actor, String.Format("Command completed in {0} milliseconds.", (endTime - startTime).TotalMilliseconds));
            }
        }
    }
}
