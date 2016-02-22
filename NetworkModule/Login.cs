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
                    var client = actor.GetPropertyOrDefault<Client>("client");
                    if (client == null) return SharpRuleEngine.PerformResult.Stop;

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
            if (existingAccount == null || Accounts.VerifyAccount(existingAccount, Password) == false)
            {
                MudObject.SendMessage(Actor, "Could not verify account.");
                return;
            }

            var client = Actor.GetPropertyOrDefault<Client>("client");
            LoginCommandHandler.LogPlayerIn(client as NetworkClient, existingAccount);
        }
	}
}
