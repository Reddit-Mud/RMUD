class entrail_quest : RMUD.Quest
{
    public override void Initialize()
    {
        Short = "entrails";
    }

    public bool Active = false;

    public override bool IsAvailable(RMUD.Actor To)
    {
        //The quest is available as long as it's not active. Simple enough.
        if (!RMUD.Mud.IsVisibleTo(To, RMUD.Mud.GetObject("palantine/soranus"))) return false;
        return !Active;
    }

    public override bool IsComplete(RMUD.Actor Questor)
    {
        var wolf = RMUD.Mud.GetObject("palantine/wolf");
        return wolf.QueryQuestProperty("is-fed");
    }

    public override void OnAccept(RMUD.Actor Questor)
    {
        Active = true;
        RMUD.Mud.SendMessage(Questor, "You have accepted the entrail quest.");

        var entrails = RMUD.Mud.GetObject("palantine/entrails");
        if (entrails.Location == null)
        {
            RMUD.MudObject.Move(entrails, Questor);
            RMUD.Mud.SendMessage(Questor, "Soranus gives you some entrails.");
        }
    }

    public override void OnCompletion(RMUD.Actor Questor)
    {
        RMUD.Mud.SendMessage(Questor, "Entrail quest completed.");
        RMUD.Mud.GetObject("palantine/wolf").ResetQuest(this);
        RMUD.Mud.GetObject("palantine/soranus").ResetQuest(this);
        Active = false;
    }

    public override void OnAbandon(RMUD.Actor Questor)
    {
        RMUD.Mud.GetObject("palantine/wolf").ResetQuest(this);
        RMUD.Mud.GetObject("palantine/soranus").ResetQuest(this);
        Active = false;
    }
}