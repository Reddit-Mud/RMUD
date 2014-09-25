using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Kick : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("KICK", false),
                    new ObjectMatcher("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None)),
                new KickProcessor(),
                "Pow! Right in the kisser!");
        }
    }

    internal class KickProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var player = Match.Arguments["PLAYER"] as Actor;

            if (player.ConnectedClient != null)
            {
                player.ConnectedClient.Send(Actor.Short + " has removed you from the server.\r\n");
                player.ConnectedClient.Disconnect();
                Mud.SendEventMessage(Actor, EventMessageScope.AllConnectedPlayers, Actor.Short + " has removed " + player.Short + " from the server.\r\n");
            }
            else if (Actor.ConnectedClient != null)
            {
                Actor.ConnectedClient.Send(player.Short + " is not connected.. what? Please find a calculator and verify that 2 + 2 = 5, as expected.\r\n");
            }
        }
    }
}
