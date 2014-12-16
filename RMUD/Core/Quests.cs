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
                var status = quest.CheckQuestStatus(player);
                var qevent = QuestEvents.Abandoned;
                if (status == QuestStatus.Completed) qevent = QuestEvents.Completed;
                else if (status == QuestStatus.Abandoned) qevent = QuestEvents.Abandoned;
                else if (status == QuestStatus.Impossible) qevent = QuestEvents.Impossible;
                else qevent = QuestEvents.Impossible;

                if (status != QuestStatus.InProgress)
                {
                    player.ActiveQuest = null;
                    quest.HandleQuestEvent(qevent, player);
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
