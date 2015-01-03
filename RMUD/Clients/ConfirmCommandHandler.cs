using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class ConfirmCommandHandler : ClientCommandHandler
	{
		public ClientCommandHandler ParentHandler;
        public CommandParser.MatchedCommand CheckedCommand;

		public ConfirmCommandHandler(
            Actor Actor, 
            CommandParser.MatchedCommand CheckedCommand, 
            ClientCommandHandler ParentHandler)
		{
            this.ParentHandler = ParentHandler;
            this.CheckedCommand = CheckedCommand;

            MudObject.SendMessage(Actor, "Are you sure you want to do that? (Y/N)");
		}

        public void HandleCommand(Actor Actor, String Command)
        {
            //Whatever the outcome of the confirmation, command handling should continue as normal afterwards.
            Actor.CommandHandler = ParentHandler;

            if (Command.ToUpper() == "YES" || Command.ToUpper() == "Y")
                Core.ProcessPlayerCommand(CheckedCommand.Command, CheckedCommand.Matches[0], Actor);
            else
                MudObject.SendMessage(Actor, "Okay, aborted.");            
        }
    }
}
