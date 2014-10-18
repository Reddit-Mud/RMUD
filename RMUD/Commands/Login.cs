using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Login : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("LOGIN", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username.\r\n")),
                new LoginProcessor(),
                "Login to an existing account.\r\n");
        }
	}

	internal class LoginProcessor : AuthenticationCommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor != null)
            {
                Mud.SendMessage(Actor.ConnectedClient, "You are already logged in.\r\n");
                return;
            }

            var client = Match.Arguments["CLIENT"] as Client;
            var userName = Match.Arguments["USERNAME"].ToString();

            client.CommandHandler = new PasswordCommandHandler(client, this, userName);
        }

        public void Authenticate(Client Client, String UserName, String Password)
        {
            var existingAccount = Mud.FindAccount(UserName);
            if (existingAccount == null || Mud.VerifyAccount(existingAccount, Password) == false)
            {
                Mud.SendMessage(Client, "Could not verify account.\r\n");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, existingAccount);
        }
	}
}
