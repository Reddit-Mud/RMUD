using RMUD;
using ConversationModule;

namespace Space
{
    public class DanConversation0 : RMUD.NPC 
    {
        public int TopicsDiscussed = 0;
        public int ConversationPhase = 0;

        public override void Initialize()
        {
            Short = "Dan";

            //Override the rule that stops the player conversing when the current interlocutor is not visible.
            Check<MudObject, DanConversation0>("can converse?").Do((actor, dan) => CheckResult.Allow);

            this.Response("ask if he is hurt", (actor, dan, topic) =>
            {
                SendMessage(actor, "No. I don't think so, anyway. I think I hit my head in the crash, but my helmet isn't cracked.");
                OnTopicDiscussed(actor);
                return PerformResult.Stop;
            }).Available((actor, dan, topic) => ConversationPhase == 0 && topic.GetPropertyOrDefault<bool>("discussed", false) == false);

            this.Response("ask where he is", (actor, dan, topic) =>
                {
                    SendMessage(actor, "I'm in a storage cabinet. I think.");
                    OnTopicDiscussed(actor);
                    return PerformResult.Stop;
                }).Available((actor, dan, topic) => ConversationPhase == 0);

            this.Response("tell him you're fine", (actor, dan, topic) =>
                {
                    SendMessage(actor, "Okay Sal..  I think I'll need your help finding the escape pod.");
                    ConversationPhase = 2;
                    return PerformResult.Stop;
                }).Available((actor, dan, topic) => ConversationPhase == 1);

            this.Response("tell him you don't know", (actor, dan, topic) =>
                {
                    SendMessage(actor, "I'll find you. I'll just need a little help.");
                    ConversationPhase = 2;
                    return PerformResult.Stop;
                }).Available((actor, dan, topic) => ConversationPhase == 1);


            this.Response("tell Dan to look around", (actor, dan, topic) =>
                {
                    Move(actor, GetObject("Start"));
                    Core.EnqueuActorCommand(actor as Actor, "LOOK");
                    Game.BlockingConversation = false;
                    Game.SuppressTopics = true;
                    return PerformResult.Stop;
                }).Available((actor, dan, topic) => ConversationPhase == 2);
        }

        private void OnTopicDiscussed(MudObject Player)
        {
            TopicsDiscussed += 1;

            if (TopicsDiscussed == 2)
            {
                SendMessage(Player, "Where are you Sal?");
                ConversationPhase = 1;
            }
        }
    }
}
