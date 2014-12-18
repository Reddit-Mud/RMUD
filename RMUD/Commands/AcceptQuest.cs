using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class AcceptQuest : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("ACCEPT"),
                    KeyWord("QUEST")),
                "Accept an offered quest.")
                .ProceduralRule((match, actor) =>
                {
                    var player = actor as Player;
                    if (player == null || player.OfferedQuest == null)
                    {
                        Mud.SendMessage(actor, "Nobody has offered you a quest.");
                        return PerformResult.Stop;
                    }

                    if (!GlobalRules.ConsiderValueRule<bool>("quest available?", player.OfferedQuest, player, player.OfferedQuest))
                    {
                        Mud.SendMessage(actor, "The quest is no longer available.");
                        player.OfferedQuest = null;
                        return PerformResult.Stop;
                    }

                    if (player.ActiveQuest != null)
                        GlobalRules.ConsiderPerformRule("quest abandoned", player.ActiveQuest, player, player.ActiveQuest);

                    player.ActiveQuest = player.OfferedQuest;
                    player.OfferedQuest = null;
                    GlobalRules.ConsiderPerformRule("quest accepted", player.ActiveQuest, player, player.ActiveQuest);

                    return PerformResult.Continue;
                });
        }
    }
}