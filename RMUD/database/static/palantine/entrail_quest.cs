class entrail_quest : RMUD.Quest
{
    public override void Initialize()
    {
        Short = "entrails";

        bool Active = false;

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest available?").Do((actor, quest) => !Active && RMUD.MudObject.IsVisibleTo(actor, RMUD.MudObject.GetObject("palantine/soranus")));

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest complete?").Do((actor, quest) =>
        {
            var wolf = RMUD.MudObject.GetObject("palantine/wolf");
            return RMUD.GlobalRules.ConsiderValueRule<bool>("entrail-quest-is-fed", wolf);
        });

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest failed?").Do((actor, quest) => !RMUD.MudObject.ObjectContainsObject(actor, RMUD.MudObject.GetObject("palantine/entrails")));

        Perform<RMUD.MudObject, RMUD.MudObject>("quest accepted").Do((questor, quest) =>
            {
                Active = true;
                RMUD.MudObject.SendMessage(questor, "You have accepted the entrail quest.");

                var entrails = RMUD.MudObject.GetObject("palantine/entrails");
                if ((RMUD.MudObject.GetObject("palantine/soranus") as RMUD.Actor).Contains(entrails, RMUD.RelativeLocations.Worn))
                {
                    RMUD.MudObject.Move(entrails, questor);
                    RMUD.MudObject.SendMessage(questor, "^<the0> gives you some entrails.", RMUD.MudObject.GetObject("palantine/soranus"));
                }
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest completed").Do((questor, quest) =>
            {
                RMUD.MudObject.SendMessage(questor, "Entrail quest completed.");
                ResetObject(RMUD.MudObject.GetObject("palantine/wolf"));
                ResetObject(RMUD.MudObject.GetObject("palantine/soranus"));
                Active = false;
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest failed").Do((questor, quest) =>
            {
                RMUD.MudObject.SendMessage(questor, "Entrail quest failed.");
                ResetObject(RMUD.MudObject.GetObject("palantine/wolf"));
                ResetObject(RMUD.MudObject.GetObject("palantine/soranus"));
                RMUD.MudObject.Move(RMUD.MudObject.GetObject("palantine/entrails"), RMUD.MudObject.GetObject("palantine/soranus"), RMUD.RelativeLocations.Worn);
                Active = false;
                return RMUD.PerformResult.Continue;
            });
    }
}