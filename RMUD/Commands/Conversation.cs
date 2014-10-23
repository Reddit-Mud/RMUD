using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
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
            if (State.Next == null || Context.ExecutingActor.CurrentInterlocutor == null) return r;

            foreach (var topic in Context.ExecutingActor.CurrentInterlocutor.ConversationTopics)
            {
                if (!topic.IsAvailable(Context.ExecutingActor, Context.ExecutingActor.CurrentInterlocutor)) continue;

                var possibleMatch = new PossibleMatch(State.Arguments, State.Next);
                bool matched = false;
                while (possibleMatch.Next != null && topic.KeyWords.Contains(possibleMatch.Next.Value.ToUpper()))
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

        public String Emit() { return "[TOPIC]"; }
    }

    internal class Conversation : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("GREET"),
                        new KeyWord("HELLO")),
                    new FailIfNoMatches(
                        new ObjectMatcher("LOCUTOR", new InScopeObjectSource(), (actor, mudobject) =>
                            {
                                if (mudobject is NPC) return MatchPreference.VeryLikely;
                                else return MatchPreference.VeryUnlikely;
                            }),
                        "Whom do you want to greet?")),
                null,
                "Enter into conversation with an NPC.");

            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("ASK"),
                        new KeyWord("TELL")),
                    new KeyWord("ABOUT", true),
                    new FailIf((pm, c) => c.ExecutingActor.CurrentInterlocutor == null,
                        "You aren't talking to anyone."),
                    new Or(
                        new TopicMatcher("TOPIC"),
                        new Rest("STRING-TOPIC"))),
                null,
                "Discuss something with someone.");

        }
    }

}
