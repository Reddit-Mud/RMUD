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
            Client.Player = Mud.GetAccountCharacter(Account);
            Client.Player.ConnectedClient = Client;
            Client.CommandHandler = Mud.ParserCommandHandler;
            Client.Rank = 500;

            Account.LoggedInCharacter = Client.Player;
 
            Mud.FindChatChannel("OOC").Subscribers.Add(Client); //Everyone is on ooc!
            MudObject.Move(Client.Player,
                Mud.GetObject(
                    (Mud.GetObject("settings") as Settings).NewPlayerStartRoom,
                    s => Mud.SendMessage(Client, s + "\r\n")));
            Mud.EnqueuClientCommand(Client, "look");
        }

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

            Parser.AddCommand(
                new Sequence(
                    new KeyWord("REGISTER", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username."),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.")),
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
                        var newCharacter = Mud.CreateCharacter(newAccount, userName);
                        newAccount.LoggedInCharacter = newCharacter; //Hackity hack.
                        LoginCommandHandler.LogPlayerIn(client, newAccount);
                    }),
                    "Create a new account.");

            Parser.AddCommand(
                new Sequence(
                    new KeyWord("LOGIN", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username."),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.")),
                new CommandProcessorWrapper((Match, Actor) =>
                {
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
				"Login to an existing account.");
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
