using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class PasswordCommandHandler : ClientCommandHandler
	{
		public ClientCommandHandler ParentHandler;
        public String UserName;
        public AuthenticationCommandProcessor AuthenticatingCommand;

        public PasswordCommandHandler(Client Client, AuthenticationCommandProcessor AuthenticatingCommand, String UserName)
		{
            this.ParentHandler = Client.CommandHandler;
            this.AuthenticatingCommand = AuthenticatingCommand;
            this.UserName = UserName;

            Mud.SendMessage(Client, "Password: ");
            Client.Echo = Echo.Mask; // TODO: Allow config setting to set this to Echo.None for extra security
		}

        public void HandleCommand(Client Client, String Password)
        {
            Client.Echo = Echo.All;
            Client.CommandHandler = ParentHandler;
            AuthenticatingCommand.Authenticate(Client, UserName, Password);
        }
    }
}
