using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum QuestStatus
    {
        Unavailable,
        Available,
        InProgress,
        Completed,
        Impossible,
        Abandoned
    }

    public enum QuestEvents
    {
        Accepted,
        Completed,
        Abandoned,
        Impossible
    }

    public class QuestRules : DeclaresRules
    {
        public void InitializeGlobalRules()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("reset-quest", "[quest, thing] : The quest is being reset. Quests can call this on objects they interact with.");
        }
    }

    public class Quest : MudObject
    {
        public Player ActiveQuestor { get; set; }
       
        public virtual QuestStatus CheckQuestStatus(Actor To) { return QuestStatus.Available; }
        public virtual void HandleQuestEvent(QuestEvents Event, Actor Questor) { }

        public void ResetObject(MudObject Thing)
        {
            GlobalRules.ConsiderPerformRule("reset-quest", Thing, this, Thing);
        }
    }
}
