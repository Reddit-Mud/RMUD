using RMUD;

namespace Wells
{
    public class Thrad : RMUD.NPC
    {
        public override void Initialize()
        {
            SimpleName("Thrad");

            Perform<MudObject, MudObject>("describe in locale")
                .When((actor, item) => item == this)
                .Do((actor, item) =>
                {
                    SendMessage(actor, "A massive knight stands in the middle of the little room.");
                    return PerformResult.Continue;
                });
        }
    }   
}