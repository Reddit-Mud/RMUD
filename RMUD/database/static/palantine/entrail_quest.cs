class entrail_quest : RMUD.MudObject
{
    public override void Initialize()
    {
        Short = "entrails";

        bool Active = false;

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest available?").Do((actor, quest) => !Active && IsVisibleTo(actor, RMUD.MudObject.GetObject("palantine/soranus")));

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest complete?").Do((actor, quest) =>
        {
            var wolf = GetObject("palantine/wolf");
            return ConsiderValueRule<bool>("entrail-quest-is-fed", wolf);
        });

        Value<RMUD.MudObject, RMUD.MudObject, bool>("quest failed?").Do((actor, quest) => !ObjectContainsObject(actor, GetObject("palantine/entrails")));

        Perform<RMUD.MudObject, RMUD.MudObject>("quest accepted").Do((questor, quest) =>
            {
                Active = true;
                SendMessage(questor, "You have accepted the entrail quest.");

                var entrails = GetObject("palantine/entrails");
                if ((GetObject("palantine/soranus") as RMUD.Actor).Contains(entrails, RMUD.RelativeLocations.Worn))
                {
                    Move(entrails, questor);
                    SendMessage(questor, "^<the0> gives you some entrails.", GetObject("palantine/soranus"));
                }
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest completed").Do((questor, quest) =>
            {
                SendMessage(questor, "Entrail quest completed.");
                this.ResetQuestObject(GetObject("palantine/wolf"));
                this.ResetQuestObject(GetObject("palantine/soranus"));
                Active = false;
                return RMUD.PerformResult.Continue;
            });

        Perform<RMUD.MudObject, RMUD.MudObject>("quest failed").Do((questor, quest) =>
            {
                SendMessage(questor, "Entrail quest failed.");
                this.ResetQuestObject(GetObject("palantine/wolf"));
                this.ResetQuestObject(GetObject("palantine/soranus"));
                Move(GetObject("palantine/entrails"), GetObject("palantine/soranus"), RMUD.RelativeLocations.Worn);
                Active = false;
                return RMUD.PerformResult.Continue;
            });
    }
}