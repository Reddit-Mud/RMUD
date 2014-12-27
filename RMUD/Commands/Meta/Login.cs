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
            if (existingAccount == null || Mud.VerifyAccount(existingAccount, Password) == false)
            {
                Mud.SendMessage(Client, "Could not verify account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Client, existingAccount);
        }
	}
}
