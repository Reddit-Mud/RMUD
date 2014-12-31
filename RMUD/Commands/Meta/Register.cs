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
                        SingleWord("USERNAME"))))
                .Manual("If you got this far, you know how to register.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor != null)
                    {
                        MudObject.SendMessage(actor, "You are already logged in.");
                        return PerformResult.Stop;
                    }

                    var client = match["CLIENT"] as Client;
                    var userName = match["USERNAME"].ToString();

                    client.CommandHandler = new PasswordCommandHandler(client, Authenticate, userName);
                    return PerformResult.Continue;
                });
        }

        public void Authenticate(Client Client, String UserName, String Password)
        {
            var existingAccount = Core.FindAccount(UserName);
            if (existingAccount != null)
            {
                MudObject.SendMessage(Client, "Account already exists.");
                return;
            }

            var newAccount = Core.CreateAccount(UserName, Password);
            if (newAccount == null)
            {
                MudObject.SendMessage(Client, "Could not create account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, newAccount);
        }
    }
}
