using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
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
                    if (actor.ConnectedClient == null) return PerformResult.Stop;

                    if (actor.ConnectedClient is NetworkClient && (actor.ConnectedClient as NetworkClient).IsLoggedOn)
                    {
                        MudObject.SendMessage(actor, "You are already logged in.");
                        return PerformResult.Stop;
                    }

                    var userName = match["USERNAME"].ToString();

                    actor.CommandHandler = new PasswordCommandHandler(actor, Authenticate, userName);
                    return PerformResult.Continue;
                });
        }

        public void Authenticate(Actor Actor, String UserName, String Password)
        {
            var existingAccount = Accounts.LoadAccount(UserName);
            if (existingAccount == null || Accounts.VerifyAccount(existingAccount, Password) == false)
            {
                MudObject.SendMessage(Actor, "Could not verify account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Actor.ConnectedClient as NetworkClient, existingAccount);
        }
	}
}
