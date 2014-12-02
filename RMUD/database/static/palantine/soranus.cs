class soranus : RMUD.NPC
{
    public override void Initialize()
    {
        AddConversationTopic("who he is", (actor, npc, topic) =>
            {
                RMUD.Mud.SendLocaleMessage(actor, "\"I am Soranus,\" <the0> says.", this);
                RMUD.Introduction.Introduce(npc);
            });

        var entrailID = AddConversationTopic("the entrails", "\"These things?\" <the0> asks. \"Nothing special. They're for the wolves.\"");
        
        var wolfID = AddConversationTopic("wolves", (actor, npc, topic) =>
        {
            RMUD.Mud.SendLocaleMessage(actor, "^<the0> grins, expossing a pair of wicked yellow canines. \"Oh don't worry, they aren't here now.\"", this);
            var quest = RMUD.Mud.GetObject("palantine/entrail_quest") as RMUD.Quest;
            if (quest.CheckQuestStatus(actor) == RMUD.QuestStatus.Available)
            {
                RMUD.Mud.SendMessage(actor, "\"Would you mind feeding them for me?\" <the0> asks.", this);
                RMUD.Mud.OfferQuest(actor, quest);
            }
        },
        (actor, npc, topic) => RMUD.Conversation.HasKnowledgeOfTopic(actor, npc, entrailID));

        DefaultResponse = new RMUD.ConversationTopic("default", "\"This is my default response,\" <the0> says, showing his sharp little teeth.");

        Short = "Soranus";
        //DescriptiveName = "short man";
        Long = "He is a man of average height, perhaps just a little short. He wears the entrails of some animal draped around himself like a toga, and his grin is full of sharp little teeth.";

        Nouns.Add("soranus", a => RMUD.Introduction.ActorKnowsActor(a, this));
    }
}