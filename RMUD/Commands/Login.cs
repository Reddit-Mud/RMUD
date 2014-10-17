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
                        "You must supply a username.\r\n"),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.\r\n")),
                new LoginProcessor(),
                "Login to an existing account.\r\n");
        }
	}

	internal class LoginProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor != null)
            {
                Mud.SendMessage(Actor.ConnectedClient, "You are already logged in.\r\n");
                return;
            }

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
        }        
	}
}
