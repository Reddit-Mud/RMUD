using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Tell : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("TELL", false),
                        new KeyWord("WHISPER", false)),
                    new KeyWord("TO", true),
                    new FailIfNoMatches(
                        new ObjectMatcher("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None),
                        "Whom?\r\n"),
                    new FailIfNoMatches(
                        new Rest("SPEECH"),
                        "And what would you like to tell then, hmm?\r\n")),
                new TellProcessor(),
                "Tell a player someMudObject privately.");
        }
	}

	internal class TellProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Object.ReferenceEquals(Actor, Match.Arguments["PLAYER"]))
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "Talking to yourself?\r\n");
                return;
            }

            var speechBuilder = new StringBuilder();
            speechBuilder.Append("[privately{0}] ");
            speechBuilder.Append(Actor.Short);
            speechBuilder.Append(": \"");
            Mud.AssembleText(Match.Arguments["SPEECH"] as LinkedListNode<String>, speechBuilder);
            speechBuilder.Append("\"\r\n");

            var player = Match.Arguments["PLAYER"] as Actor;
            if (player.ConnectedClient != null)
                Mud.SendMessage(player, String.Format(speechBuilder.ToString(), ""));
            if (Actor.ConnectedClient != null)
            {
                Mud.SendMessage(Actor, String.Format(speechBuilder.ToString(), " to " + player.Short));
                if (player.ConnectedClient != null && player.ConnectedClient.IsAfk)
                    Mud.SendMessage(Actor, String.Format("[{0} is AFK: {1}]", player.Short, player.ConnectedClient.Account.AFKMessage));
            }
        }        
	}
}
