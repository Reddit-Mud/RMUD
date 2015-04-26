using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
    /// <summary>
    /// A ClientCommandHandler that uses the parser defined in Core to process player input.
    /// </summary>
	public class ParserCommandHandler : ClientCommandHandler
	{
        public void HandleCommand(PendingCommand Command)
        {
            if (String.IsNullOrEmpty(Command.RawCommand)) return;

            bool displayMatches = false;
            bool displayTime = false;

            // Debug commands always start with '@'.
            if (Command.RawCommand[0] == '@')
            {
                if (Command.RawCommand.ToUpper().StartsWith("@MATCH "))
                {
                    // Display all matches of the player's input. Do not actually execute the command.
                    Command.RawCommand = Command.RawCommand.Substring("@MATCH ".Length);
                    displayMatches = true;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@TIME "))
                {
                    // Time how long the command takes to match and execute.
                    Command.RawCommand = Command.RawCommand.Substring("@TIME ".Length);
                    displayTime = true;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@DEBUG "))
                {
                    // Turn off the automatic command execution timeout while executing the command. 
                    // Use this when testing a command in the debugger. Otherwise, the command processing thread might
                    // be aborted while you are debugging it.
                    Command.RawCommand = Command.RawCommand.Substring("@DEBUG ".Length);
                    if (Command.Actor.Rank < 500)
                    {
                        MudObject.SendMessage(Command.Actor, "You do not have sufficient rank to use the debug command.");
                        return;
                    }

                    // This will be reset by the command queue before the next command is parsed.
                    Core.CommandTimeoutEnabled = false;
                }
                else if (Command.RawCommand.ToUpper().StartsWith("@RULES "))
                {
                    // Display all the rules invoked while executing this command.
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

            var matchedCommand = Core.DefaultParser.ParseCommand(Command);

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
                    // If there are multiple matches, replace this handler with a disambiguation handler.
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
