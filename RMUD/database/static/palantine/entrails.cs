class entrails : RMUD.MudObject
{
    public override void Initialize()
    {
        Short = "entrails";
        Nouns.Add("entrails");

        Perform<RMUD.MudObject, RMUD.MudObject>("on-dropped").Do((actor, item) =>
            {
                var wolf = RMUD.Mud.GetObject("palantine/wolf");
                if (wolf.Location == actor.Location)
                {
                    RMUD.GlobalRules.ConsiderPerformRule("handle-entrail-drop", wolf, this);
                    return RMUD.PerformResult.Stop;
                }
                return RMUD.PerformResult.Continue;
            });
    }
}