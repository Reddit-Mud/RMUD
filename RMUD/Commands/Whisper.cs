using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Whisper : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("WHISPER"),
                    new KeyWord("TO", true),
                    new FailIfNoMatches(
                        new ObjectMatcher("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None),
                        "Whom?"),
                    new FailIfNoMatches(
                        new Rest("SPEECH"),
                        "And what would you like to tell them, hmm?")),
                new TellProcessor(),
                "Tell a player something privately.");
        }
	}

	internal class TellProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Object.ReferenceEquals(Actor, Match.Arguments["PLAYER"]))
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "Talking to yourself?");
                return;
            }

            var speechBuilder = new StringBuilder();
            speechBuilder.Append("[privately{0}] ");
            speechBuilder.Append(Actor.Short);
            speechBuilder.Append(": \"");
            speechBuilder.Append(Match.Arguments["SPEECH"].ToString());
            speechBuilder.Append("\"");

            var player = Match.Arguments["PLAYER"] as Actor;
            if (player.ConnectedClient != null)
            {
                if (player.ConnectedClient.IsAfk)
                    Mud.SendMessage(player, String.Format("{1} " + speechBuilder.ToString(), "", DateTime.Now));
                else
                    Mud.SendMessage(player, String.Format(speechBuilder.ToString(), ""));
            }
            if (Actor.ConnectedClient != null)
            {
                Mud.SendMessage(Actor, String.Format(speechBuilder.ToString(), " to " + player.Short));
                if (player.ConnectedClient != null && player.ConnectedClient.IsAfk)
                    Mud.SendMessage(Actor, String.Format("[{0} is AFK: {1}]", player.Short, player.ConnectedClient.Account.AFKMessage));
            }
        }        
	}
}
