using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class ParserCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

		public ParserCommandHandler(CommandParser Parser)
		{
            this.Parser = Parser;
		}

        public void HandleCommand(Actor Actor, String Command)
        {
            if (String.IsNullOrEmpty(Command)) return;

            bool displayMatches = false;
            bool displayTime = false;

            if (Command[0] == '@')
            {
                if (Command.ToUpper().StartsWith("@MATCH "))
                {
                    Command = Command.Substring("@MATCH ".Length);
                    displayMatches = true;
                }
                else if (Command.ToUpper().StartsWith("@TIME "))
                {
                    Command = Command.Substring("@TIME ".Length);
                    displayTime = true;
                }
                else if (Command.ToUpper().StartsWith("@DEBUG "))
                {
                    Command = Command.Substring("@DEBUG ".Length);
                    if (Actor.Rank < 500)
                    {
                        MudObject.SendMessage(Actor, "You do not have sufficient rank to use the debug command.");
                        return;
                    }

                    Core.CommandTimeoutEnabled = false;
                }
                else if (Command.ToUpper().StartsWith("@RULES "))
                {
                    Command = Command.Substring("@RULES ".Length);
                    Core.GlobalRules.LogRules(Actor);
                }
                else
                {
                    MudObject.SendMessage(Actor, "I don't recognize that debugging command.");
                    return;
                }
            }

            var startTime = DateTime.Now;

            var matchedCommand = Parser.ParseCommand(Command, Actor);

            if (displayMatches)
            {
                var matchEndTime = DateTime.Now;

                if (matchedCommand == null)
                {
                    MudObject.SendMessage(Actor, String.Format("Matched nothing in {0:n0} milliseconds.",
                        (matchEndTime - startTime).TotalMilliseconds));
                }
                else
                {
                    MudObject.SendMessage(Actor, String.Format("Matched {0} in {1:n0} milliseconds. {2} unique matches.",
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

                        MudObject.SendMessage(Actor, builder.ToString());
                    }
                }
            }
            else
            {

                if (matchedCommand != null)
                {
                    if (matchedCommand.Matches.Count > 1)
                        Actor.CommandHandler = new DisambigCommandHandler(Actor, matchedCommand, this);
                    else
                        Core.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], Actor);
                }
                else
                    MudObject.SendMessage(Actor, "huh?");
            }

            Core.GlobalRules.LogRules(null);

            if (displayTime)
            {
                var endTime = DateTime.Now;

                MudObject.SendMessage(Actor, String.Format("Command completed in {0} milliseconds.", (endTime - startTime).TotalMilliseconds));
            }
        }
    }
}
