class entrails : Clothing
{
    public override void Initialize()
    {
        Short = "entrails";
        Nouns.Add("entrails");
        Layer = ClothingLayer.Over;
        BodyPart = ClothingBodyPart.Cloak;
        Article = "some";

        Perform<MudObject, MudObject>("drop").Do((actor, item) =>
            {
                var wolf = GetObject("palantine/wolf");
                if (wolf.Location == actor.Location)
                {
                    ConsiderPerformRule("handle-entrail-drop", wolf, this);
                    return PerformResult.Stop;
                }
                return PerformResult.Continue;
            });
    }
}