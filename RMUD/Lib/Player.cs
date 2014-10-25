using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Player : Actor
	{
        public Quest ActiveQuest { get; set; }

        public Dictionary<String, System.Collections.BitArray> ConversationKnowledge = new Dictionary<String, System.Collections.BitArray>();
        public NPC CurrentInterlocutor = null;

        public bool HasKnowledgeOfTopic(NPC Locutor, int TopicID)
        {
            if (TopicID < 0) throw new InvalidOperationException();
            System.Collections.BitArray knowledgeArray;
            if (!ConversationKnowledge.TryGetValue(Locutor.Path, out knowledgeArray)) return false;
            if (knowledgeArray.Length <= TopicID) return false;
            return knowledgeArray[TopicID];
        }

        public void GrantKnowledgeOfTopic(NPC Locutor, int TopicID)
        {
            if (TopicID < 0) throw new InvalidOperationException();

            System.Collections.BitArray knowledgeArray;
            if (!ConversationKnowledge.TryGetValue(Locutor.Path, out knowledgeArray))
            {
                knowledgeArray = new System.Collections.BitArray(8);
                ConversationKnowledge.Upsert(Locutor.Path, knowledgeArray);
            }

            if (knowledgeArray.Length <= TopicID) knowledgeArray.Length = TopicID + 1;
            knowledgeArray[TopicID] = true;
        }
	}
}
