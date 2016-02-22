using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
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
                    var client = actor.GetPropertyOrDefault<Client>("client");
                    if (client is NetworkClient && (client as NetworkClient).IsLoggedOn)
                    {
                        MudObject.SendMessage(actor, "You are already logged in.");
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    var userName = match["USERNAME"].ToString();

                    actor.SetProperty("command handler", new PasswordCommandHandler(actor, Authenticate, userName));
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }

        public void Authenticate(MudObject Actor, String UserName, String Password)
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

            var client = Actor.GetPropertyOrDefault<Client>("client");
            LoginCommandHandler.LogPlayerIn(client as NetworkClient, newAccount);
        }
    }
}
