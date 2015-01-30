using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RMUD;

namespace NetworkModule
{
	public class LoginCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

        public static void LogPlayerIn(NetworkClient Client, Account Account)
        {
            bool newSession = false;

            if (Account.LoggedInCharacter != null)
            {
                //Connect to the existing session
                if (Account.LoggedInCharacter.ConnectedClient != null)
                {
                    Account.LoggedInCharacter.ConnectedClient.Player = null;
                    Account.LoggedInCharacter.ConnectedClient.Send("You are being disconnected because you have logged into this account from another connection.\r\n");
                    Account.LoggedInCharacter.ConnectedClient.Disconnect();
                }
                Client.Send("You were already logged in. You are being connected to that session.\r\n");
                Client.Player = Account.LoggedInCharacter;
            }
            else
            {
                //Start a new session
                Client.Player = Accounts.GetAccountCharacter(Account);
                newSession = true;
            }

            Client.Player.SetProperty("account", Account);
            Client.IsLoggedOn = true;
            Client.Player.CommandHandler = Core.ParserCommandHandler;
            Account.LoggedInCharacter = Client.Player;
            Core.TiePlayerToClient(Client, Client.Player);

            if (newSession)
                Core.AddPlayer(Client.Player);
        }

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

            CommandFactory.CreateCommandFactory(typeof(Login)).Create(Parser);
            CommandFactory.CreateCommandFactory(typeof(Register)).Create(Parser);
            CommandFactory.CreateCommandFactory(typeof(Quit)).Create(Parser);

		}

		public void HandleCommand(Actor Actor, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, null);
                if (matchedCommand != null)
                {
                    Core.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], Actor);
                }
                else
                    MudObject.SendMessage(Actor, "I do not understand.");
			}
			catch (Exception e)
			{
				Core.ClearPendingMessages();
                MudObject.SendMessage(Actor, e.Message);
			}
		}
	}
}
