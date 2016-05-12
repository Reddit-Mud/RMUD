class entrail_quest : MudObject
{
    public override void Initialize()
    {
        Short = "entrails";

        bool Active = false;

        Value<MudObject, MudObject, bool>("quest available?").Do((actor, quest) => !Active && IsVisibleTo(actor, MudObject.GetObject("palantine/soranus")));

        Value<MudObject, MudObject, bool>("quest complete?").Do((actor, quest) =>
        {
            var wolf = GetObject("palantine/wolf");
            return ConsiderValueRule<bool>("entrail-quest-is-fed", wolf);
        });

        Value<MudObject, MudObject, bool>("quest failed?").Do((actor, quest) => !ObjectContainsObject(actor, GetObject("palantine/entrails")));

        Perform<MudObject, MudObject>("quest accepted").Do((questor, quest) =>
            {
                Active = true;
                SendMessage(questor, "You have accepted the entrail quest.");

                var entrails = GetObject("palantine/entrails");
                if ((GetObject("palantine/soranus") as Actor).Contains(entrails, RelativeLocations.Worn))
                {
                    Move(entrails, questor);
                    SendMessage(questor, "^<the0> gives you some entrails.", GetObject("palantine/soranus"));
                }
                return PerformResult.Continue;
            });

        Perform<MudObject, MudObject>("quest completed").Do((questor, quest) =>
            {
                SendMessage(questor, "Entrail quest completed.");
                this.ResetQuestObject(GetObject("palantine/wolf"));
                this.ResetQuestObject(GetObject("palantine/soranus"));
                Active = false;
                return PerformResult.Continue;
            });

        Perform<MudObject, MudObject>("quest failed").Do((questor, quest) =>
            {
                SendMessage(questor, "Entrail quest failed.");
                this.ResetQuestObject(GetObject("palantine/wolf"));
                this.ResetQuestObject(GetObject("palantine/soranus"));
                Move(GetObject("palantine/entrails"), GetObject("palantine/soranus"), RelativeLocations.Worn);
                Active = false;
                return PerformResult.Continue;
            });
    }
}