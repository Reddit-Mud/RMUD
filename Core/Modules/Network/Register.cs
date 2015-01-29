using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Network
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
            if (existingAccount != null)
            {
                MudObject.SendMessage(Actor, "Account already exists.");
                return;
            }

            var newAccount = Accounts.CreateAccount(UserName, Password);
            if (newAccount == null)
            {
                MudObject.SendMessage(Actor, "Could not create account.");
                return;
            }

            LoginCommandHandler.LogPlayerIn(Actor.ConnectedClient as NetworkClient, newAccount);
        }
    }
}
