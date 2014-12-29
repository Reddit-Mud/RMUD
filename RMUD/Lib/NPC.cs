using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class NPC : Actor
	{
        public List<ConversationTopic> ConversationTopics = new List<ConversationTopic>();
        public ConversationTopic DefaultResponse;

        public int AddConversationTopic(ConversationTopic Topic)
        {
            Topic.ID = ConversationTopics.Count;
            ConversationTopics.Add(Topic);
            return Topic.ID;
        }

        public int AddConversationTopic(String Topic, String Response, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, Response, AvailabilityRule));
        }

        public int AddConversationTopic(String Topic, Func<Actor, MudObject, String> LambdaResponse, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, LambdaResponse, AvailabilityRule));
        }

        public int AddConversationTopic(String Topic, Action<Actor, NPC, ConversationTopic> SilentResponse, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            return AddConversationTopic(new ConversationTopic(Topic, SilentResponse, AvailabilityRule));
        }

        public void Wear(MudObject Item)
        {
            Mud.Move(Item, this, RelativeLocations.Worn);
        }

        public void Wear(String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            Wear(Clothing.Create(Short, Layer, BodyPart));
        }
	}
}
