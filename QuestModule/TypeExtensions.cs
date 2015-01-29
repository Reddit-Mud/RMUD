using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using RMUD;

namespace QuestModule
{
    public static class Extensions
    {
        public static void OfferQuest(this MudObject This, Actor Actor, MudObject Quest)
        {
            var player = Actor as Player;
            if (player != null)
            {
                MudObject.SendMessage(Actor, "[To accept this quest, enter the command 'accept quest'.]");
                if (player.GetProperty<MudObject>("active-quest") != null)
                    MudObject.SendMessage(Actor, "[Accepting this quest will abandon your active quest.]");
                player.SetProperty("offered-quest", Quest);
            }
        }

        public static void ResetQuestObject(this MudObject This, MudObject Thing)
        {
            Core.GlobalRules.ConsiderPerformRule("quest reset", This, Thing);
        }
    }
}
