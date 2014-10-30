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

    public class Quest : MudObject
    {
        public Player ActiveQuestor { get; set; }
       
        public virtual QuestStatus CheckQuestStatus(Actor To) { return QuestStatus.Available; }
        public virtual void HandleQuestEvent(QuestEvents Event, Actor Questor) { }
    }
}
