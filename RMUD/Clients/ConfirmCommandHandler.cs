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
            Client Client, 
            CommandParser.MatchedCommand CheckedCommand, 
            ClientCommandHandler ParentHandler)
		{
            this.ParentHandler = ParentHandler;
            this.CheckedCommand = CheckedCommand;

            Mud.SendMessage(Client, "Are you sure you want to do that? (Y/N)");
		}

        public void HandleCommand(Client Client, String Command)
        {
            //Whatever the outcome of the confirmation, command handling should continue as normal afterwards.
            Client.CommandHandler = ParentHandler;

            if (Command.ToUpper() == "YES" || Command.ToUpper() == "Y")
                CheckedCommand.Command.Processor.Perform(CheckedCommand.Matches[0], Client.Player);
            else
                Mud.SendMessage(Client, "Okay, aborted.");            
        }
    }
}
