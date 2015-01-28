using RMUD;
using RMUD.Modules.Conversation;

namespace Space
{
    public class Dan : RMUD.NPC
    {
        public override void Initialize()
        {
            Short = "Dan";

            var conversationPhase = 0;

            this.Response("if he is hurt", (actor, dan, topic) =>
            {
                SendMessage(actor, "No. I don't think so, anyway. I think I hit my head in the crash, but my helmet isn't cracked.");
                conversationPhase = 1;
                return PerformResult.Continue;
            }).Available((actor, dan, topic) => conversationPhase == 0);
        }
    }
}
