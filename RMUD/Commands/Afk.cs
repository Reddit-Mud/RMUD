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
                Sequence(
                    KeyWord("AFK"),
                    MustMatch("You have to supply an afk message.",
                        Rest("MESSAGE"))),
                "Set your afk message.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient != null)
                        actor.ConnectedClient.Account.AFKMessage = Mud.RestText(match.Arguments["MESSAGE"]);
                    Mud.SendMessage(actor, "AFK message set.");
                    return PerformResult.Continue;
                });
        }
	}
}
