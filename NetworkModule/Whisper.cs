using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
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
                    if (System.Object.ReferenceEquals(actor, match["PLAYER"]))
                    {
                        MudObject.SendMessage(actor, "Talking to yourself?");
                        return SharpRuleEngine.PerformResult.Stop;
                    }
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .ProceduralRule((match, actor) =>
                {
                    var player = match["PLAYER"] as Actor;
                    MudObject.SendMessage(player, "[privately " + DateTime.Now + "] ^<the0> : \"" + match["SPEECH"].ToString() + "\"", actor);
                    MudObject.SendMessage(actor, "[privately to <the0>] ^<the1> : \"" + match["SPEECH"].ToString() + "\"", player, actor);
                    if (player.ConnectedClient is NetworkClient && (player.ConnectedClient as NetworkClient).IsAfk)
                        MudObject.SendMessage(actor, "^<the0> is afk : " + player.ConnectedClient.Player.GetProperty<Account>("account").AFKMessage, player);
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
	}
}
