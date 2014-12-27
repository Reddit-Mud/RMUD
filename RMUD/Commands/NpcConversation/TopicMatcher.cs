using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Topic(String CaptureName)
        {
            return new TopicMatcher(CaptureName);
        }
    }

    public class TopicMatcher : CommandTokenMatcher
    {
        public String CaptureName;

        public TopicMatcher(String CaptureName)
        {
            this.CaptureName = CaptureName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
            if (State.Next == null) return r;
            if (!(Context.ExecutingActor is Player)) return r;

            NPC locutor = null;
            if (State.Arguments.ContainsKey("NEW-LOCUTOR")) locutor = State.Arguments["NEW-LOCUTOR"] as NPC;
            else locutor = (Context.ExecutingActor as Player).CurrentInterlocutor;
            if (locutor == null) return r;

            foreach (var topic in locutor.ConversationTopics)
            {
                if (!topic.IsAvailable(Context.ExecutingActor as Player, locutor)) continue;

                var possibleMatch = State.Clone();
                bool matched = false;
                while (possibleMatch.Next != null && topic.KeyWords.Match(possibleMatch.Next.Value.ToUpper(), Context.ExecutingActor))
                {
                    matched = true;
                    possibleMatch.Next = possibleMatch.Next.Next;
                }

                if (matched)
                {
                    possibleMatch.Arguments.Upsert(CaptureName, topic);
                    r.Add(possibleMatch);
                }
            }

            return r;
        }

        public String FindFirstKeyWord() { return null; }
        public String Emit() { return "[TOPIC]"; }
    }
}
