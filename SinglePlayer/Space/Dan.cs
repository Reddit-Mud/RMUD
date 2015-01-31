using RMUD;
using ConversationModule;

namespace Space
{
    public class Dan : RMUD.NPC 
    {
        public override void Initialize()
        {
            Short = "Dan";

            //Override the rule that stops the player conversing when the current interlocutor is not visible.
            Check<MudObject, Dan>("can converse?").Do((actor, dan) => CheckResult.Allow);

            var conversationPhase = 0;

            this.Response("if he is hurt", (actor, dan, topic) =>
            {
                SendMessage(actor, "No. I don't think so, anyway. I think I hit my head in the crash, but my helmet isn't cracked.");
                conversationPhase = 1;
                return PerformResult.Stop;
            }).Available((actor, dan, topic) => conversationPhase == 0);

            this.Response("to take a look around", (actor, dan, topic) =>
                {
                    Move(actor, GetObject("Start"));
                    Core.EnqueuActorCommand(actor as Actor, "LOOK");
                    conversationPhase = 2;
                    Game.BlockingConversation = false;
                    Game.SuppressTopics = true;
                    return PerformResult.Stop;
                }).Available((actor, dan, topic) => conversationPhase == 1);
        }
    }
}
