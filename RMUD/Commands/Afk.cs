using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class AFK : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("AFK", false),
                    new FailIfNoMatches(
                        new Rest("MESSAGE"),
                        "You have to supply an afk message.\r\n")),
                new AFKProcessor(),
                "Set your afk message.");
        }
	}

	internal class AFKProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var messageBuilder = new StringBuilder();
            Mud.AssembleText(Match.Arguments["MESSAGE"] as LinkedListNode<String>, messageBuilder);
            if (Actor.ConnectedClient != null)
                Actor.ConnectedClient.Account.AFKMessage = messageBuilder.ToString();
            Mud.SendMessage(Actor, "AFK message set.\r\n");
        }        
	}
}
