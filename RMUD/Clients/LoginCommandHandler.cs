using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class LoginCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

        public static void LogPlayerIn(Client Client, Account Account)
        {
            Client.Account = Account;
            Client.CommandHandler = Mud.ParserCommandHandler;
            Client.Rank = 500;

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
                Client.Player = Mud.GetAccountCharacter(Account);
                MudObject.Move(Client.Player, 
                    Mud.GetObject(
                        Mud.SettingsObject.NewPlayerStartRoom,
                        s => Mud.SendMessage(Client, s)));
                Mud.EnqueuClientCommand(Client, "look");
            }

            Mud.FindChatChannel("OOC").Subscribers.Add(Client); //Everyone is on ooc!
            Client.Player.ConnectedClient = Client;
            Account.LoggedInCharacter = Client.Player;
        }

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

            CommandFactory.GetCommand(typeof(Commands.Login)).Create(Parser);
            CommandFactory.GetCommand(typeof(Commands.Register)).Create(Parser);
            CommandFactory.GetCommand(typeof(Commands.Quit)).Create(Parser);

		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, null);
                if (matchedCommand != null)
                {
                    matchedCommand.Matches[0].Arguments.Upsert("CLIENT", Client);
                    Mud.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], null);
                }
                else
                    Mud.SendMessage(Client, "I do not understand.");
			}
			catch (Exception e)
			{
				Mud.ClearPendingMessages();
                Mud.SendMessage(Client, e.Message);
			}
		}
	}
}
