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
                Sequence(
                    KeyWord("LOGIN"),
                    MustMatch("You must supply a username.",
                        SingleWord("USERNAME"))))
                .Manual("If you got this far, you know how to login.")
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
            var existingAccount = MudObject.FindAccount(UserName);
            if (existingAccount == null || MudObject.VerifyAccount(existingAccount, Password) == false)
            {
                MudObject.SendMessage(Client, "Could not verify account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, existingAccount);
        }
	}
}
