using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class TopicSource : IObjectSource
    {
        public String LocutorArgument;

        public TopicSource() { }
        public TopicSource(String LocutorArgument)
        {
            this.LocutorArgument = LocutorArgument;
        }

        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            NPC source = null;
            if (!String.IsNullOrEmpty(LocutorArgument))
                source = State[LocutorArgument] as NPC;
            else if (Context.ExecutingActor is Player && (Context.ExecutingActor as Player).CurrentInterlocutor != null)
                source = (Context.ExecutingActor as Player).CurrentInterlocutor;

            if (source != null)
                return source.ConversationTopics;
            return new List<MudObject>();
        }
    }
}
