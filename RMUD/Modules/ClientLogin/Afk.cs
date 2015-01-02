using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.ClientLogin
{
	internal class AFK : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("AFK"),
                    MustMatch("You have to supply an afk message.",
                        Rest("MESSAGE"))))
                .Manual("Sets your afk message. This message is displayed after 5 minutes of inactivity on the WHO list, and to any player who attempts to whisper to you.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.ConnectedClient != null)
                        actor.ConnectedClient.Account.AFKMessage = match["MESSAGE"].ToString();
                    MudObject.SendMessage(actor, "AFK message set.");
                    return PerformResult.Continue;
                });
        }
	}
}
