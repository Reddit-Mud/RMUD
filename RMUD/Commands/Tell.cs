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
                    new ObjectMatcher("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None),
                    new Rest("SPEECH")),
                new TellProcessor(),
                "Tell a player something privately.");
        }
	}

	internal class TellProcessor : ICommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Object.ReferenceEquals(Actor, Match.Arguments["PLAYER"]))
            {
                if (Actor.ConnectedClient != null) Actor.ConnectedClient.Send("Talking to yourself?\r\n");
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
                player.ConnectedClient.Send(String.Format(speechBuilder.ToString(), ""));
            if (Actor.ConnectedClient != null)
                Actor.ConnectedClient.Send(String.Format(speechBuilder.ToString(), " to " + player.Short));
        }        
	}
}
