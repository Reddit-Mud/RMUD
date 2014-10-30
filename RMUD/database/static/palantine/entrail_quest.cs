class entrail_quest : RMUD.Quest
{
    public override void Initialize()
    {
        Short = "entrails";
    }

    public bool Active = false;

    public override RMUD.QuestStatus CheckQuestStatus(RMUD.Actor To)
    {
        if (!Active)
        {
            if (RMUD.Mud.IsVisibleTo(To, RMUD.Mud.GetObject("palantine/soranus"))) return RMUD.QuestStatus.Available;
            else return RMUD.QuestStatus.Unavailable;
        }

        if (RMUD.Mud.GetObject("palantine/wolf").QueryQuestProperty("is-fed") == true) return RMUD.QuestStatus.Completed;

        var entrails = RMUD.Mud.GetObject("palantine/entrails");
        if (entrails.Location == null) return RMUD.QuestStatus.Impossible;
        if (!RMUD.Mud.ObjectContainsObject(To, entrails)) return RMUD.QuestStatus.Abandoned;
        return RMUD.QuestStatus.InProgress;
    }

    public override void HandleQuestEvent(RMUD.QuestEvents Event, RMUD.Actor Questor)
    {
        switch (Event)
        {
            case RMUD.QuestEvents.Accepted:
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
                break;
            case RMUD.QuestEvents.Completed:
                RMUD.Mud.SendMessage(Questor, "Entrail quest completed.");
                RMUD.Mud.GetObject("palantine/wolf").ResetQuest(this);
                RMUD.Mud.GetObject("palantine/soranus").ResetQuest(this);
                Active = false;
                break;
            case RMUD.QuestEvents.Abandoned:
                RMUD.Mud.GetObject("palantine/wolf").ResetQuest(this);
                RMUD.Mud.GetObject("palantine/soranus").ResetQuest(this);
                RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine/entrails"), null);
                Active = false;
                break;
            case RMUD.QuestEvents.Impossible:
                Active = false;
                break;
        }
    }
}