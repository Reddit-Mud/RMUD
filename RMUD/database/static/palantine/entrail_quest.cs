class entrail_quest : RMUD.Quest
{
    public override void Initialize()
    {
        Short = "entrails";

        bool Active = false;

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest available?").Do((actor, quest) => !Active && RMUD.Mud.IsVisibleTo(actor, RMUD.Mud.GetObject("palantine/soranus")));

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest complete?").Do((actor, quest) =>
        {
            var wolf = RMUD.Mud.GetObject("palantine/wolf");
            return RMUD.GlobalRules.ConsiderValueRule<bool>("entrail-quest-is-fed", wolf, wolf);
        });

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest failed?").Do((actor, quest) => !RMUD.Mud.ObjectContainsObject(actor, RMUD.Mud.GetObject("palantine/entrails")));

        Perform<RMUD.MudObject, RMUD.MudObject>("quest accepted").Do((questor, quest) =>
            {
                Active = true;
                RMUD.Mud.SendMessage(questor, "You have accepted the entrail quest.");

                var entrails = RMUD.Mud.GetObject("palantine/entrails");
                if ((RMUD.Mud.GetObject("palantine/soranus") as RMUD.Actor).Contains(entrails, RMUD.RelativeLocations.Worn))
                {
                    RMUD.MudObject.Move(entrails, questor);
                    RMUD.Mud.SendMessage(questor, "^<the0> gives you some entrails.", RMUD.Mud.GetObject("palantine/soranus"));
                }
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest completed").Do((questor, quest) =>
            {
                RMUD.Mud.SendMessage(questor, "Entrail quest completed.");
                ResetObject(RMUD.Mud.GetObject("palantine/wolf"));
                ResetObject(RMUD.Mud.GetObject("palantine/soranus"));
                Active = false;
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest failed").Do((questor, quest) =>
            {
                RMUD.Mud.SendMessage(questor, "Entrail quest failed.");
                ResetObject(RMUD.Mud.GetObject("palantine/wolf"));
                ResetObject(RMUD.Mud.GetObject("palantine/soranus"));
                RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine/entrails"), RMUD.Mud.GetObject("palantine/soranus"), RMUD.RelativeLocations.Worn);
                Active = false;
                return RMUD.PerformResult.Continue;
            });
    }
}