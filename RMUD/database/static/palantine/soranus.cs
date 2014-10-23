class soranus : RMUD.NPC
{
    public override void Initialize()
    {
        AddConversationTopic("who he is", "\"I am Soranus,\" Soranus says.");
        var entrailID = AddConversationTopic("the entrails", "\"These things?\" Soranus asks. \"Nothing special. They're for the wolves.\"");
        var wolfID = AddConversationTopic("wolves", "Soranus grins, expossing a pair of wicked yellow canines. \"Oh don't worry, they aren't here now.\"", (actor, npc, topic) => actor.HasKnowledgeOfTopic(npc, entrailID));

        DefaultResponse = new RMUD.ConversationTopic("default", "\"This is my default response,\" Soranus says, showing his sharp little teeth.");

        Short = "Soranus";
        Long = "Soranus is a man of average height, perhaps just a little short. He wears the entrails of some animal draped around himself like a toga, and his grin is full of sharp little teeth.";

        Nouns.Add("soranus");
    }
}