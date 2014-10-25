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
        public Func<Player, NPC, ConversationTopic, bool> AvailabilityRule;
        public ResponseTypes ResponseType;
        public DescriptiveText NormalResponse;
        public Action<Player, NPC, ConversationTopic> SilentResponse;

        public bool IsAvailable(Player Actor, NPC NPC)
        {
            if (AvailabilityRule == null) return true;
            return AvailabilityRule(Actor, NPC, this);
        }

        private void SetKeywords(String From)
        {
            var keywords = From.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            KeyWords = new NounList(keywords);
        }

        public ConversationTopic(String Topic, String Response, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Normal;
            this.NormalResponse = Response;
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }

        public ConversationTopic(String Topic, Func<Actor, MudObject, String> LambdaResponse, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Normal;
            this.NormalResponse = new DescriptiveText(LambdaResponse);
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }

        public ConversationTopic(String Topic, Action<Actor, NPC, ConversationTopic> SilentResponse, Func<Player, NPC, ConversationTopic, bool> AvailabilityRule = null)
        {
            this.ResponseType = ResponseTypes.Silent;
            this.SilentResponse = SilentResponse;
            this.Topic = Topic;
            this.AvailabilityRule = AvailabilityRule;
            SetKeywords(Topic);
        }
    }
}
