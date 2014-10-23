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

        public static void GreetLocutor(Actor Actor, NPC Whom)
        {
            //Todo: Greeting rules?
            //Todo: NPC Greeting response

            Actor.CurrentInterlocutor = Whom;
        }

        public static void DiscussTopic(Actor Actor, NPC With, ConversationTopic Topic)
        {
            Mud.SendMessage(Actor, String.Format("You discuss '{0}' with {1}.", Topic.Topic, Actor.CurrentInterlocutor.Definite));
            Mud.SendExternalMessage(Actor, String.Format("{0} discusses '{1}' with {2}.", Actor.Definite, Topic.Topic, Actor.CurrentInterlocutor.Definite));

            if (Topic.ResponseType == ConversationTopic.ResponseTypes.Normal)
            {
                var response = Topic.NormalResponse.Expand(Actor, Actor.CurrentInterlocutor);
                Mud.SendLocaleMessage(Actor, response);
            }
            else if (Topic.ResponseType == ConversationTopic.ResponseTypes.Silent)
            {
                Topic.SilentResponse(Actor, Actor.CurrentInterlocutor, Topic);
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
                        (mo as Actor).GrantKnowledgeOfTopic(Actor.CurrentInterlocutor, Topic.ID);
                    return EnumerateObjectsControl.Continue;
                });

            ConversationHelper.ListSuggestedTopics(Actor, Actor.CurrentInterlocutor);
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

            ConversationHelper.GreetLocutor(Actor, locutor as NPC);
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
                    ConversationHelper.GreetLocutor(Actor, newInterlocutor);
            }

            if (Actor.CurrentInterlocutor == null)
            {
                Mud.SendMessage(Actor, "You aren't talking to anyone.");
                return;
            }

            if (!Match.Arguments.ContainsKey("TOPIC"))
            {
                if (Actor.CurrentInterlocutor.DefaultResponse != null)
                    ConversationHelper.DiscussTopic(Actor, Actor.CurrentInterlocutor, Actor.CurrentInterlocutor.DefaultResponse);
                else
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

            ConversationHelper.DiscussTopic(Actor, Actor.CurrentInterlocutor, topic);
        }
    }
}
