using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Network
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
                    var clients = Clients.ConnectedClients.Where(c => c is NetworkClient && (c as NetworkClient).IsLoggedOn);
                    MudObject.SendMessage(actor, "~~ THESE PLAYERS ARE ONLINE NOW ~~");
                    foreach (NetworkClient client in clients)
                        MudObject.SendMessage(actor,
                            "[" + Core.SettingsObject.GetNameForRank(client.Player.Rank) + "] <a0> ["
                            + client.ConnectionDescription + "]"
                            + (client.IsAfk ? (" afk: " + client.Account.AFKMessage) : "")
                            + (client.Player.Location != null ? (" -- " + client.Player.Location.Path) : ""),
                            client.Player);
                    return PerformResult.Continue;
                });
        }
	}
}
