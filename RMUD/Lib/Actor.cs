using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : GenericContainer, TakeRules
	{
		public Client ConnectedClient;

		public override string Definite { get { return Short; } }
		public override string Indefinite { get { return Short; } }

        CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        { }

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
