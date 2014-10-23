using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class NPC : Actor
	{
        public List<ConversationTopic> ConversationTopics = new List<ConversationTopic>();

        public void AddConversationTopic(ConversationTopic Topic)
        {
            Topic.ID = ConversationTopics.Count;
            ConversationTopics.Add(Topic);
        }
	}
}
