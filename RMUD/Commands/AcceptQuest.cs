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
                    KeyWord("QUEST")))
                .Name("ACCEPT QUEST")
                .Manual("When an NPC offers you a quest, you will be prompted to accept the quest. Accepting a quest abandons any quest you already have active.")
                .ProceduralRule((match, actor) =>
                {
                    if (!(actor is Player) || (actor as Player).OfferedQuest == null)
                    {
                        Mud.SendMessage(actor, "Nobody has offered you a quest.");
                        return PerformResult.Stop;
                    }
                    else
                    {
                        match.Upsert("QUEST", (actor as Player).OfferedQuest);
                        return PerformResult.Continue;
                    }
                }, "the must have been offered a quest, and bookeeping rule.")
                .ProceduralRule((match, actor) =>
                {
                    var player = actor as Player;
                    if (!GlobalRules.ConsiderValueRule<bool>("quest available?", player, player.OfferedQuest))
                    {
                        Mud.SendMessage(actor, "The quest is no longer available.");
                        player.OfferedQuest = null;
                        return PerformResult.Stop;
                    }
                    return PerformResult.Continue;
                }, "the quest must be available rule.")
                .ProceduralRule((match, actor) =>
                {
                    var player = actor as Player;
                    if (player.ActiveQuest != null)
                        GlobalRules.ConsiderPerformRule("quest abandoned", player, player.ActiveQuest);

                    player.ActiveQuest = player.OfferedQuest;
                    player.OfferedQuest = null;
                    return PerformResult.Continue;
                }, "the any active quest must be abandoned rule.")
                .Perform("quest accepted", "ACTOR", "QUEST");
        }
    }
}