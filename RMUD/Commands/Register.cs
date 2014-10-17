using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Register : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("REGISTER", false),
                    new FailIfNoMatches(
                        new SingleWord("USERNAME"),
                        "You must supply a username.\r\n"),
                    new FailIfNoMatches(
                        new SingleWord("PASSWORD"),
                        "You must supply a password.\r\n")),
                    new RegistrationProcessor(),
                    "Create a new account.\r\n");
        }
	}

	internal class RegistrationProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
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
        }
    }
}
