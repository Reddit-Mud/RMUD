class soranus : RMUD.NPC
{
    public override void Initialize()
    {
        Response("who he is", (actor, npc, topic) =>
            {
                SendLocaleMessage(actor, "\"I am Soranus,\" <the0> says.", this);
                Introduce(this);
                return RMUD.PerformResult.Stop;
            });

        Response("the entrails", "\"These things?\" <the0> asks. \"Nothing special. They're for the wolves.\"");
        
        Response("wolves", (actor, npc, topic) =>
        {
            SendLocaleMessage(actor, "^<the0> grins, expossing a pair of wicked yellow canines. \"Oh don't worry, they aren't here now.\"", this);
            var quest = GetObject("palantine/entrail_quest") as RMUD.Quest;
            if (ConsiderValueRule<bool>("quest available?", actor, quest))
            {
                SendMessage(actor, "\"Would you mind feeding them for me?\" <the0> asks.", this);
                OfferQuest(actor as RMUD.Actor, quest);
            }
            return RMUD.PerformResult.Stop;
        });

        Perform<RMUD.MudObject, RMUD.MudObject, RMUD.MudObject>("topic response")
            .When((actor, npc, topic) => topic == null)
            .Do((actor, npc, topic) =>
            {
                SendLocaleMessage(actor, "\"This is my default response,\" <the0> says, showing his sharp little teeth.", this);
                return RMUD.PerformResult.Stop;
            });

        Short = "Soranus";

        Nouns.Add("soranus", a => ActorKnowsActor(a, this));

        Wear("toga", RMUD.ClothingLayer.Outer, RMUD.ClothingBodyPart.Torso);
        Wear(GetObject("palantine/entrails"));
    }
}