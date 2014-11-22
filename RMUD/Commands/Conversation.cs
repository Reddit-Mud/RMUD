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

        public String Emit() { return "[TOPIC]"; }
    }

    internal class ConversationCommandFactory : CommandFactory
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

            Conversation.GreetLocutor(Actor as Player, locutor as NPC);
            Conversation.ListSuggestedTopics(Actor as Player, locutor as NPC);
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
                    Conversation.GreetLocutor(Actor as Player, newInterlocutor);
            }

            if ((Actor as Player).CurrentInterlocutor == null)
            {
                Mud.SendMessage(Actor, "You aren't talking to anyone.");
                return;
            }

            if (!Match.Arguments.ContainsKey("TOPIC"))
            {
                if ((Actor as Player).CurrentInterlocutor.DefaultResponse != null)
                    Conversation.DiscussTopic((Actor as Player), (Actor as Player).CurrentInterlocutor, (Actor as Player).CurrentInterlocutor.DefaultResponse);
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

            Conversation.DiscussTopic((Actor as Player), (Actor as Player).CurrentInterlocutor, topic);
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
