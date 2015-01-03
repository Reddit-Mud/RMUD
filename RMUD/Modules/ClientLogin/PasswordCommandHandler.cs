using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD.Modules.ClientLogin
{
	public class PasswordCommandHandler : ClientCommandHandler
	{
		public ClientCommandHandler ParentHandler;
        public String UserName;
        public Action<Actor, String, String> AuthenticatingCommand;

        public PasswordCommandHandler(Actor Actor, Action<Actor, String, String> AuthenticatingCommand, String UserName)
		{
            this.ParentHandler = Actor.CommandHandler;
            this.AuthenticatingCommand = AuthenticatingCommand;
            this.UserName = UserName;

            MudObject.SendMessage(Actor, "Password: ");
            Actor.ConnectedClient.Echo = Echo.Mask; // TODO: Allow config setting to set this to Echo.None for extra security
		}

        public void HandleCommand(Actor Actor, String Password)
        {
            Actor.ConnectedClient.Echo = Echo.All;
            Actor.CommandHandler = ParentHandler;
            AuthenticatingCommand(Actor, UserName, Password);
        }
    }
}
