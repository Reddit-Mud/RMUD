using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace NetworkModule
{
    public class Stats
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<MudObject>("enumerate-stats")
                .Do((actor) =>
                {
                    MudObject.SendMessage(actor, "CLIENTS");
                    return SharpRuleEngine.PerformResult.Continue;
                });

            GlobalRules.Perform<MudObject, String>("stats")
                .When((actor, type) => type == "CLIENTS")
                .Do((actor, type) =>
                {
                    MudObject.SendMessage(actor, "~~ CLIENTS ~~");
                    foreach (var client in Clients.ConnectedClients)
                        if (client is NetworkClient)
                            MudObject.SendMessage(actor, (client as NetworkClient).ConnectionDescription + (client.Player == null ? "" : (" - " + client.Player.GetProperty<String>("Short"))));
                        else
                            MudObject.SendMessage(actor, "local " + (client.Player == null ? "" : (" - " + client.Player.GetProperty<String>("Short"))));
                    return SharpRuleEngine.PerformResult.Stop;
                });
        }
    }
}
