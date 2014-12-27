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
                Sequence(
                    Or(
                        KeyWord("WHISPER"),
                        KeyWord("TELL")),
                    OptionalKeyWord("TO"),
                    MustMatch("Whom?",
                        Object("PLAYER", new ConnectedPlayersObjectSource(), ObjectMatcherSettings.None)),
                    MustMatch("Tell them what?", Rest("SPEECH"))))
                .Manual("Sends a private message to the player of your choice.")
                .ProceduralRule((match, actor) =>
                {
                    if (System.Object.ReferenceEquals(actor, match.Arguments["PLAYER"]))
                    {
                        Mud.SendMessage(actor, "Talking to yourself?");
                        return PerformResult.Stop;
                    }
                    return PerformResult.Continue;
                })
                .ProceduralRule((match, actor) =>
                {
                    var player = match.Arguments["PLAYER"] as Actor;
                    Mud.SendMessage(player, "[privately " + DateTime.Now + "] ^<the0> : \"" + match.Arguments["SPEECH"].ToString() + "\"", actor);
                    Mud.SendMessage(actor, "[privately to <the0>] ^<the1> : \"" + match.Arguments["SPEECH"].ToString() + "\"", player, actor);
                    if (player.ConnectedClient != null && player.ConnectedClient.IsAfk)
                        Mud.SendMessage(actor, "^<the0> is afk : " + player.ConnectedClient.Account.AFKMessage, player);
                    return PerformResult.Continue;
                });
        }
	}
}
