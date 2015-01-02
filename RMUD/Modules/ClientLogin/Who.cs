using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.ClientLogin
{
	internal class Who : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                KeyWord("WHO"))
                .Manual("Displays a list of current logged in players.")
                .ProceduralRule((match, actor) =>
                {
                    var clients = Core.ConnectedClients.Where(c => c.IsLoggedOn);
                    MudObject.SendMessage(actor, "~~ THESE PLAYERS ARE ONLINE NOW ~~");
                    foreach (var client in clients)
                        MudObject.SendMessage(actor,
                            "[" + Core.SettingsObject.GetNameForRank(client.Rank) + "] <a0> ["
                            + client.ConnectionDescription + "]"
                            + (client.IsAfk ? (" afk: " + client.Account.AFKMessage) : "")
                            + (client.Player.Location != null ? (" -- " + client.Player.Location.Path) : ""),
                            client.Player);
                    return PerformResult.Continue;
                });
        }
	}
}
