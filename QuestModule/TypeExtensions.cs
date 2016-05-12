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
        public static void OfferQuest(this MudObject This, MudObject Actor, MudObject Quest)
        {
            if (Actor != null)
            {
                MudObject.SendMessage(Actor, "[To accept this quest, enter the command 'accept quest'.]");
                if (Actor.GetProperty<MudObject>("active-quest") != null)
                    MudObject.SendMessage(Actor, "[Accepting this quest will abandon your active quest.]");
                Actor.SetProperty("offered-quest", Quest);
            }
        }

        public static void ResetQuestObject(this MudObject This, MudObject Thing)
        {
            Core.GlobalRules.ConsiderPerformRule("quest reset", This, Thing);
        }
    }
}
