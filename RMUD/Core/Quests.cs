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
        public static void CheckForQuestCompletion(Actor Actor)
        {
            var player = Actor as Player;
            if (player != null && player.ActiveQuest != null)
            {
                var quest = player.ActiveQuest;
                if (quest.IsComplete(player))
                {
                    player.ActiveQuest = null;
                    quest.OnCompletion(player);
                }
            }
        }

        public static void OfferQuest(Actor Actor, Quest Quest, String Message)
        {
            var player = Actor as Player;
            if (player != null)
            {
                SendMessage(Actor, Message);
                SendMessage(Actor, "[To accept this quest, enter the command 'accept quest'.]");
                if (player.ActiveQuest != null)
                    SendMessage(Actor, "[Accepting this quest will abandon your active quest.]");
                player.OfferedQuest = Quest;
            }
        }
    }
}
