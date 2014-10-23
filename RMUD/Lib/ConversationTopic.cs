using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class ConversationTopic
    {
        public enum ResponseTypes
        {
            Normal,
            Silent
        }

        public int ID;
        public NounList KeyWords;
        public String Topic;
        public Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule;
        public ResponseTypes ResponseType;
        public DescriptiveText NormalResponse;
        public Action<Actor, NPC, ConversationTopic> SilentResponse;

        public bool IsAvailable(Actor Actor, NPC NPC)
        {
            if (AvailabilityRule == null) return true;
            return AvailabilityRule(Actor, NPC, this);
        }

        private void SetKeywords(String From)
        {
            var keywords = From.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            KeyWords = new NounList(keywords);
        }

        public ConversationTopic(String Topic, String Response, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Normal;
            this.NormalResponse = Response;
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }

        public ConversationTopic(String Topic, Func<Actor, MudObject, String> LambdaResponse, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Normal;
            this.NormalResponse = new DescriptiveText(LambdaResponse);
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }

        public ConversationTopic(String Topic, Action<Actor, NPC, ConversationTopic> SilentResponse, Func<Actor, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Silent;
            this.SilentResponse = SilentResponse;
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }
    }
}
