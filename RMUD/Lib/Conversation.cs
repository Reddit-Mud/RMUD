using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class Conversation
    {
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
