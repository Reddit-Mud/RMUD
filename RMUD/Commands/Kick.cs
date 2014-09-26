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

            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("KICK", false),
                    new SingleWord("MASK")),
                new MaskedKickProcessor(),
                "FAAAAAALCON PUNCH!");
        }
    }

    internal class KickProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            Kick(Match.Arguments["PLAYER"] as Actor, Actor);
        }

        public static void Kick(Actor Player, Actor Actor)
        {
            if (Player.ConnectedClient != null)
            {
                Mud.SendMessage(Player, Actor.Short + " has removed you from the server.\r\n");
                Player.ConnectedClient.Disconnect();
                Mud.SendMessage(Actor, MessageScope.AllConnectedPlayers, Actor.Short + " has removed " + Player.Short + " from the server.\r\n");
            }
        }
    }

    internal class MaskedKickProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var mask = Match.Arguments["MASK"].ToString();
            var maskRegex = new System.Text.RegularExpressions.Regex(ProscriptionList.ConvertGlobToRegex(mask), System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            //Iterate over local copy because kicking modifies ConnectedClients.
            foreach (var client in new List<Client>(Mud.ConnectedClients))
            {
                if (client.IsLoggedOn && maskRegex.Matches(client.IPString).Count > 0)
                    KickProcessor.Kick(client.Player, Actor);
            }
        }
    }
}
