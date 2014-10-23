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
            if (State.Next == null) return r;

            NPC locutor = null;
            if (State.Arguments.ContainsKey("NEW-LOCUTOR")) locutor = State.Arguments["NEW-LOCUTOR"] as NPC;
            else locutor = Context.ExecutingActor.CurrentInterlocutor;
            if (locutor == null) return r;

            foreach (var topic in locutor.ConversationTopics)
            {
                if (!topic.IsAvailable(Context.ExecutingActor, locutor)) continue;

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
                new GreetProcessor(),
                "Enter into conversation with an NPC.");

            //Todo: Implicitely greet
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("ASK"),
                        new KeyWord("TELL")),
                    new Optional(
                        new ObjectMatcher("NEW-LOCUTOR", new InScopeObjectSource(), (Actor, mudobject) =>
                            {
                                if (Object.ReferenceEquals(mudobject, Actor.CurrentInterlocutor)) return MatchPreference.VeryLikely;
                                if (mudobject is NPC) return MatchPreference.Likely;
                                else return MatchPreference.VeryUnlikely;
                            })),
                    new KeyWord("ABOUT", true),
                    new FirstOf(
                        new TopicMatcher("TOPIC"),
                        new Rest("STRING-TOPIC"))),
                new DiscussProcessor(),
                "Discuss something with someone.");

        }
    }

    public static class ConversationHelper
    {
        public static void ListSuggestedTopics(Actor For, NPC Of)
        {
            //Get the first 4 available topics that the player hasn't already seen.
            var suggestedTopics = Of.ConversationTopics.Where(topic => topic.IsAvailable(For, Of) && !For.HasKnowledgeOfTopic(Of, topic.ID)).Take(4);
            if (suggestedTopics.Count() != 0)
                Mud.SendMessage(For, "Suggested topics: " + String.Join(", ", suggestedTopics.Select(topic => topic.Topic)) + ".");
        }

        public static void ImplicitelyGreet(Actor Actor, NPC Whom)
        {
            //Todo: Greeting rules?
            //Todo: NPC Greeting response

            Actor.CurrentInterlocutor = Whom;
        }
    }

    public class GreetProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var locutor = Match.Arguments["LOCUTOR"];
            if (!(locutor is NPC))
            {
                Mud.SendMessage(Actor, "You can't speak to that.");
                return;
            }

            if (!Mud.IsVisibleTo(Actor, locutor as NPC))
            {
                Mud.SendMessage(Actor, "They don't seem to be here.");
                return;
            }

            ConversationHelper.ImplicitelyGreet(Actor, locutor as NPC);
            ConversationHelper.ListSuggestedTopics(Actor, locutor as NPC);
        }
    }

    public class DiscussProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Match.Arguments.ContainsKey("NEW-LOCUTOR"))
            {
                var newInterlocutor = Match.Arguments["NEW-LOCUTOR"] as NPC;
                if (newInterlocutor == null)
                {
                    Mud.SendMessage(Actor, "You can't talk to that.");
                    return;
                }

                if (!Object.ReferenceEquals(newInterlocutor, Actor.CurrentInterlocutor))
                    ConversationHelper.ImplicitelyGreet(Actor, newInterlocutor);
            }

            if (Actor.CurrentInterlocutor == null)
            {
                Mud.SendMessage(Actor, "You aren't talking to anyone.");
                return;
            }

            if (!Match.Arguments.ContainsKey("TOPIC"))
            {
                //Todo: Give NPCs a default response.
                Mud.SendMessage(Actor, "That doesn't seem to be a topic I understand.");
                return;
            }
                        
            if (!Mud.IsVisibleTo(Actor, Actor.CurrentInterlocutor))
            {
                Mud.SendMessage(Actor, "They don't seem to be here anymore.");
                Actor.CurrentInterlocutor = null;
                return;
            }

            var topic = Match.Arguments["TOPIC"] as ConversationTopic;

            Mud.SendMessage(Actor, String.Format("You discuss '{0}' with {1}.", topic.Topic, Actor.CurrentInterlocutor.Definite));
            Mud.SendExternalMessage(Actor, String.Format("{0} discusses '{1}' with {2}.", Actor.Definite, topic.Topic, Actor.CurrentInterlocutor.Definite));

            if (topic.ResponseType == ConversationTopic.ResponseTypes.Normal)
            {
                var response = topic.NormalResponse.Expand(Actor, Actor.CurrentInterlocutor);
                Mud.SendLocaleMessage(Actor, response);
            }
            else if (topic.ResponseType == ConversationTopic.ResponseTypes.Silent)
            {
                topic.SilentResponse(Actor, Actor.CurrentInterlocutor, topic);
            }
            else
            {
                throw new InvalidOperationException();
            }

            var locale = Mud.FindLocale(Actor);
            if (locale != null)
                Mud.EnumerateObjects(locale, (mo, relloc) =>
                    {
                        if (mo is Actor)
                            (mo as Actor).GrantKnowledgeOfTopic(Actor.CurrentInterlocutor, topic.ID);
                        return EnumerateObjectsControl.Continue;
                    });

            ConversationHelper.ListSuggestedTopics(Actor, Actor.CurrentInterlocutor);

        }
    }
}
