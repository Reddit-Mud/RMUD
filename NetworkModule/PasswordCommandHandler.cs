using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RMUD;

namespace NetworkModule
{
	public class PasswordCommandHandler : ClientCommandHandler
	{
		public ClientCommandHandler ParentHandler;
        public String UserName;
        public Action<MudObject, String, String> AuthenticatingCommand;

        public PasswordCommandHandler(MudObject Actor, Action<MudObject, String, String> AuthenticatingCommand, String UserName)
		{
            this.ParentHandler = Actor.GetPropertyOrDefault<ClientCommandHandler>("command handler");
            this.AuthenticatingCommand = AuthenticatingCommand;
            this.UserName = UserName;

            MudObject.SendMessage(Actor, "Password: ");
		}

        public void HandleCommand(PendingCommand Command)
        {
            Command.Actor.SetProperty("command handler", ParentHandler);
            AuthenticatingCommand(Command.Actor, UserName, Command.RawCommand);
        }
    }
}
