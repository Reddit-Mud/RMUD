class entrails : RMUD.Clothing
{
    public override void Initialize()
    {
        Short = "entrails";
        Nouns.Add("entrails");
        Layer = RMUD.ClothingLayer.Over;
        BodyPart = RMUD.ClothingBodyPart.Cloak;
        Article = "some";

        Perform<RMUD.MudObject, RMUD.MudObject>("dropped").Do((actor, item) =>
            {
                var wolf = GetObject("palantine/wolf");
                if (wolf.Location == actor.Location)
                {
                    ConsiderPerformRule("handle-entrail-drop", wolf, this);
                    return RMUD.PerformResult.Stop;
                }
                return RMUD.PerformResult.Continue;
            });
    }
}