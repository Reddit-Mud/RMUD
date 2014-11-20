using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class Conversation
    {
        public static void ListSuggestedTopics(Player For, NPC Of)
        {
            //Get the first 4 available topics that the player hasn't already seen.
            var suggestedTopics = Of.ConversationTopics.Where(topic => topic.IsAvailable(For, Of) && !HasKnowledgeOfTopic(For, Of, topic.ID)).Take(4);
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
            Mud.SendMessage(Actor, String.Format("You discuss '{0}' with {1}.", Topic.Topic, Actor.CurrentInterlocutor.Definite(Actor)));

            Mud.SendExternalMessage(Actor, String.Format("<0> discusses '{0}' with <1>.", Topic.Topic), Actor, Actor.CurrentInterlocutor);

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
                        GrantKnowledgeOfTopic(mo as Player, Actor.CurrentInterlocutor, Topic.ID);
                    return EnumerateObjectsControl.Continue;
                });

            Conversation.ListSuggestedTopics(Actor, Actor.CurrentInterlocutor);
        }

        public static bool HasKnowledgeOfTopic(Player Who, NPC Locutor, int TopicID)
        {
            if (TopicID < 0) throw new InvalidOperationException();
            var knowledgeArray = Who.Recall<System.Collections.BitArray>(Locutor, "ConversationKnowledge");
            if (knowledgeArray == null) return false;
            if (knowledgeArray.Length <= TopicID) return false;
            return knowledgeArray[TopicID];
        }

        public static void GrantKnowledgeOfTopic(Player Who, NPC Locutor, int TopicID)
        {
            if (TopicID < 0) throw new InvalidOperationException();
            System.Collections.BitArray knowledgeArray = null;
            if (!Who.TryRecall(Locutor, "ConversationKnowledge", out knowledgeArray))
            {
                knowledgeArray = new System.Collections.BitArray(8);
                Who.Remember(Locutor, "ConversationKnowledge", knowledgeArray);
            }

            if (knowledgeArray.Length <= TopicID) knowledgeArray.Length = TopicID + 1;
            knowledgeArray[TopicID] = true;
        }
    }
}
