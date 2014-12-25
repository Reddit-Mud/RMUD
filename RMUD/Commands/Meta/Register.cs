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
                Sequence(
                    KeyWord("REGISTER"),
                    MustMatch("You must supply a username.",
                        SingleWord("USERNAME"))),
                "Create a new account.")
                .Manual("If you got this far, you know how to register.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor != null)
                    {
                        Mud.SendMessage(actor, "You are already logged in.");
                        return PerformResult.Stop;
                    }

                    var client = match.Arguments["CLIENT"] as Client;
                    var userName = match.Arguments["USERNAME"].ToString();

                    client.CommandHandler = new PasswordCommandHandler(client, Authenticate, userName);
                    return PerformResult.Continue;
                });
        }

        public void Authenticate(Client Client, String UserName, String Password)
        {
            var existingAccount = Mud.FindAccount(UserName);
            if (existingAccount != null)
            {
                Mud.SendMessage(Client, "Account already exists.");
                return;
            }

            var newAccount = Mud.CreateAccount(UserName, Password);
            if (newAccount == null)
            {
                Mud.SendMessage(Client, "Could not create account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, newAccount);
        }
    }
}
