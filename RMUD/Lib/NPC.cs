using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class NPC : Actor
	{
        public List<ConversationTopic> ConversationTopics = new List<ConversationTopic>();

        public int AddConversationTopic(ConversationTopic Topic)
        {
            Topic.ID = ConversationTopics.Count;
            ConversationTopics.Add(Topic);
            return Topic.ID;
        }

        public int AddConversationTopic(String Topic, String Response, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, Response, AvailabilityRule));
        }

        public int AddConversationTopic(String Topic, Func<Actor, MudObject, String> LambdaResponse, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, LambdaResponse, AvailabilityRule));
        }

        public int AddConversationTopic(String Topic, Action<Actor, NPC, ConversationTopic> SilentResponse, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, SilentResponse, AvailabilityRule));
        }
	}
}
