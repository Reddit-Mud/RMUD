using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        public static void OfferQuest(Actor Actor, MudObject Quest)
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

        public void ResetQuestObject(MudObject Thing)
        {
            GlobalRules.ConsiderPerformRule("quest reset", this, Thing);
        }
    }

    public partial class Player
    {
        public MudObject OfferedQuest { get; set; }
        public MudObject ActiveQuest { get; set; }
    }
}
