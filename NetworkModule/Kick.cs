﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
{
    internal class Kick : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("KICK"),
                    Or(
                        Object("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None),
                        SingleWord("MASK"))))
                .Manual("Makes bad people go away.")
                .ProceduralRule((match, actor) =>
                {
                    if (match.ContainsKey("PLAYER"))
                        KickPlayer(match["PLAYER"] as MudObject, actor);
                    else
                    {
                        var mask = match["MASK"].ToString();
                        var maskRegex = new System.Text.RegularExpressions.Regex(ProscriptionList.ConvertGlobToRegex(mask), System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        //Iterate over local copy because kicking modifies ConnectedClients.
                        foreach (var client in new List<Client>(Clients.ConnectedClients))
                        {
                            var netClient = client as NetworkClient;
                            if (netClient != null && netClient.IsLoggedOn && maskRegex.Matches(netClient.IPString).Count > 0)
                            {
                                Core.MarkLocaleForUpdate(client.Player);
                                KickPlayer(client.Player, actor);
                            }
                        }
                    }

                    return SharpRuleEngine.PerformResult.Continue;
                });
        }

        public static void KickPlayer(MudObject Player, MudObject Actor)
        {
            var client = Player.GetProperty<Client>("client");
            if (client != null)
            {
                Core.MarkLocaleForUpdate(Player);

                MudObject.SendMessage(Player, Actor.GetProperty<String>("short") + " has removed you from the server.");
                client.Disconnect();
                Clients.SendGlobalMessage(Actor.GetProperty<String>("short") + " has removed " + Player.GetProperty<String>("short") + " from the server.");
            }
        }
    }
}
