class entrails : RMUD.MudObject
{
    public override void Initialize()
    {
        Short = "entrails";
        Nouns.Add("entrails");

        AddActionRule<RMUD.MudObject, RMUD.MudObject>("on-dropped").Do((actor, item) =>
            {
                var wolf = RMUD.Mud.GetObject("palantine/wolf");
                if (wolf.Location == actor.Location)
                {
                    RMUD.GlobalRules.ConsiderActionRule("handle-entrail-drop", wolf, this);
                    return RMUD.RuleResult.Stop;
                }
                return RMUD.RuleResult.Continue;
            });
    }
}