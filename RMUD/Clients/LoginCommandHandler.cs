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

        private static void LogPlayerIn(Client Client, Account Account)
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
                    Account.LoggedInCharacter.ConnectedClient.Disconnect();
                }
                Client.Player = Account.LoggedInCharacter;
            }
            else
            {
                //Start a new session
                Client.Player = Mud.GetAccountCharacter(Account);
                MudObject.Move(Client.Player,
                    Mud.GetObject(
                        (Mud.GetObject("settings") as Settings).NewPlayerStartRoom,
                        s => Mud.SendMessage(Client, s + "\r\n")));
                Mud.EnqueuClientCommand(Client, "look");
            }

            Mud.FindChatChannel("OOC").Subscribers.Add(Client); //Everyone is on ooc!
            Client.Player.ConnectedClient = Client;
            Account.LoggedInCharacter = Client.Player;
        }

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

            Parser.AddCommand(
                new Sequence(
                    new KeyWord("REGISTER", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username.\r\n"),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.\r\n")),
                new CommandProcessorWrapper((Match, Actor) =>
                    {
                        var client = Match.Arguments["CLIENT"] as Client;
                        var userName = Match.Arguments["USERNAME"].ToString();
                        var password = Match.Arguments["PASSWORD"].ToString();

                        var existingAccount = Mud.FindAccount(userName);
                        if (existingAccount != null)
                        {
                            Mud.SendMessage(client, "That account already exists.\r\n");
                            return;
                        }

                        var newAccount = Mud.CreateAccount(userName, password);
                        LoginCommandHandler.LogPlayerIn(client, newAccount);
                    }),
                    "Create a new account.\r\n");

            Parser.AddCommand(
                new Sequence(
                    new KeyWord("LOGIN", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username.\r\n"),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.\r\n")),
                new CommandProcessorWrapper((Match, Actor) =>
                {
                    Mud.CommandTimeoutEnabled = false;

                    var client = Match.Arguments["CLIENT"] as Client;
                    var userName = Match.Arguments["USERNAME"].ToString();
                    var password = Match.Arguments["PASSWORD"].ToString();

                    var existingAccount = Mud.FindAccount(userName);
                    if (existingAccount == null || Mud.VerifyAccount(existingAccount, password) == false)
                    {
                        Mud.SendMessage(client, "Could not verify account.\r\n");
                        return;
                    }

                    LoginCommandHandler.LogPlayerIn(client, existingAccount);
                }),
                "Login to an existing account.\r\n");
		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, null);
                if (matchedCommand != null)
                {
                    matchedCommand.Matches[0].Arguments.Upsert("CLIENT", Client);
                    matchedCommand.Command.Processor.Perform(matchedCommand.Matches[0], null);
                }
                else
                    Mud.SendMessage(Client, "I do not understand.\r\n");
			}
			catch (Exception e)
			{
				Mud.ClearPendingMessages();
                Mud.SendMessage(Client, e.Message);
			}
		}
	}
}
