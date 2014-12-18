using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        public static void CheckQuestStatus(Actor Actor)
        {
            var player = Actor as Player;
            if (player != null && player.ActiveQuest != null)
            {
                var quest = player.ActiveQuest;

                if (GlobalRules.ConsiderValueRule<bool>("quest complete?", quest, player, quest))
                {
                    player.ActiveQuest = null;
                    GlobalRules.ConsiderPerformRule("quest completed", quest, player, quest);
                }
                else if (GlobalRules.ConsiderValueRule<bool>("quest failed?", quest, player, quest))
                {
                    player.ActiveQuest = null;
                    GlobalRules.ConsiderPerformRule("quest failed", quest, player, quest);
                }
            }
        }

        public static void OfferQuest(Actor Actor, Quest Quest)
        {
            var player = Actor as Player;
            if (player != null)
            {
                SendMessage(Actor, "[To accept this quest, enter the command 'accept quest'.]");
                if (player.ActiveQuest != null)
                    SendMessage(Actor, "[Accepting this quest will abandon your active quest.]");
                player.OfferedQuest = Quest;
            }
        }
    }
}
