class wolf : RMUD.NPC, RMUD.WitnessDropRules
{
    public bool IsFed = false;

    public override void Initialize()
    {
        DefaultResponse = new RMUD.ConversationTopic("default", "The wolf snarls and howls, showing its large sharp teeth.");

        Short = "wolf";
        Long = "This is a snarling grey beast with a long snout and upright ears. It has a tail that does not wag even a little.";

        Nouns.Add("wolf");

        RMUD.Mud.RegisterForHeartbeat(this);
    }

    public override void Heartbeat(ulong HeartbeatID)
    {
        if (!IsFed)
            RMUD.Mud.SendLocaleMessage(this, "The wolf whines for food.");
    }

    void RMUD.WitnessDropRules.Handle(RMUD.Actor Actor, RMUD.MudObject Item)
    {
        var entrails = RMUD.Mud.GetObject("palantine/entrails");
        if (System.Object.ReferenceEquals(entrails, Item))
        {
            RMUD.Mud.SendLocaleMessage(this, "The wolf snatches up the entrails.");
            IsFed = true;
            RMUD.MudObject.Move(entrails, null);
        }
    }

    public override bool QueryQuestProperty(string Name)
    {
        if (Name == "is-fed") return IsFed;
        return false;
    }

    public override void ResetQuest(RMUD.Quest Quest)
    {
        IsFed = false;
    }

}