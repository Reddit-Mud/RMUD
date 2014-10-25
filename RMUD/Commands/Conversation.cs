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
            if (!(Context.ExecutingActor is Player)) return r;

            NPC locutor = null;
            if (State.Arguments.ContainsKey("NEW-LOCUTOR")) locutor = State.Arguments["NEW-LOCUTOR"] as NPC;
            else locutor = (Context.ExecutingActor as Player).CurrentInterlocutor;
            if (locutor == null) return r;

            foreach (var topic in locutor.ConversationTopics)
            {
                if (!topic.IsAvailable(Context.ExecutingActor as Player, locutor)) continue;

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
                                if (Actor is Player && Object.ReferenceEquals(mudobject, (Actor as Player).CurrentInterlocutor)) return MatchPreference.VeryLikely;
                                if (mudobject is NPC) return MatchPreference.Likely;
                                else return MatchPreference.VeryUnlikely;
                            })),
                    new KeyWord("ABOUT", true),
                    new FirstOf(
                        new TopicMatcher("TOPIC"),
                        new Rest("STRING-TOPIC"))),
                new DiscussProcessor(),
                "Discuss something with someone.");

            Parser.AddCommand(
                new KeyWord("TOPICS"),
                new ListTopicsProcessor(),
                "List the currently available conversation topics.");
        }
    }

    public static class ConversationHelper
    {
        public static void ListSuggestedTopics(Player For, NPC Of)
        {
            //Get the first 4 available topics that the player hasn't already seen.
            var suggestedTopics = Of.ConversationTopics.Where(topic => topic.IsAvailable(For, Of) && !For.HasKnowledgeOfTopic(Of, topic.ID)).Take(4);
            if (suggestedTopics.Count() != 0)
                Mud.SendMessage(For, "Suggested topics: " + String.Join(", ", suggestedTopics.Select(topic => topic.Topic)) + ".");
        }

        public static void GreetLocutor(Player Actor, NPC Whom)
        {
            //Todo: Greeting rules?
            //Todo: NPC Greeting response

            Actor.CurrentInterlocutor = Whom;
        }

        public static void DiscussTopic(Player Actor, NPC With, ConversationTopic Topic)
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
                    if (mo is Player)
                        (mo as Player).GrantKnowledgeOfTopic(Actor.CurrentInterlocutor, Topic.ID);
                    return EnumerateObjectsControl.Continue;
                });

            ConversationHelper.ListSuggestedTopics(Actor, Actor.CurrentInterlocutor);
        }
    }

    public class GreetProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (!(Actor is Player)) return;

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

            ConversationHelper.GreetLocutor(Actor as Player, locutor as NPC);
            ConversationHelper.ListSuggestedTopics(Actor as Player, locutor as NPC);
        }
    }

    public class DiscussProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (!(Actor is Player)) return;

            if (Match.Arguments.ContainsKey("NEW-LOCUTOR"))
            {
                var newInterlocutor = Match.Arguments["NEW-LOCUTOR"] as NPC;
                if (newInterlocutor == null)
                {
                    Mud.SendMessage(Actor, "You can't talk to that.");
                    return;
                }

                if (!Object.ReferenceEquals(newInterlocutor, (Actor as Player).CurrentInterlocutor))
                    ConversationHelper.GreetLocutor(Actor as Player, newInterlocutor);
            }

            if ((Actor as Player).CurrentInterlocutor == null)
            {
                Mud.SendMessage(Actor, "You aren't talking to anyone.");
                return;
            }

            if (!Match.Arguments.ContainsKey("TOPIC"))
            {
                if ((Actor as Player).CurrentInterlocutor.DefaultResponse != null)
                    ConversationHelper.DiscussTopic((Actor as Player), (Actor as Player).CurrentInterlocutor, (Actor as Player).CurrentInterlocutor.DefaultResponse);
                else
                    Mud.SendMessage(Actor, "That doesn't seem to be a topic I understand.");
                return;
            }

            if (!Mud.IsVisibleTo(Actor, (Actor as Player).CurrentInterlocutor))
            {
                Mud.SendMessage(Actor, "They don't seem to be here anymore.");
                (Actor as Player).CurrentInterlocutor = null;
                return;
            }

            var topic = Match.Arguments["TOPIC"] as ConversationTopic;

            ConversationHelper.DiscussTopic((Actor as Player), (Actor as Player).CurrentInterlocutor, topic);
        }
    }

    public class ListTopicsProcessor : CommandProcessor
    {

        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var player = Actor as Player;
            if (player == null) return;

            if (player.CurrentInterlocutor == null)
            {
                Mud.SendMessage(Actor, "You aren't talking to anyone.");
                return;
            }

            var availableTopics = player.CurrentInterlocutor.ConversationTopics.Where(topic => topic.IsAvailable(player, player.CurrentInterlocutor));

            if (availableTopics.Count() == 0)
                Mud.SendMessage(Actor, "There are no topics to discuss.");
            else
            {
                Mud.SendMessage(Actor, "These topics are available...");
                foreach (var topic in availableTopics)
                    Mud.SendMessage(Actor, "   " + topic.Topic);
            }
        }
    }
}
