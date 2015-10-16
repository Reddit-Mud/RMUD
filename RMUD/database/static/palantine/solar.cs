public class solar : Room
{
    int TimesViewed = 0;
    string Brief;

    public override void Initialize()
    {
        RoomType = RoomType.Exterior;
        Short = "Palantine Villa - Solar";

        Long = "You are in a gorgeous solar filled with white marble and glowing sun beams. Massive windows above let in copious amounts of light that pick out flecks of minerals in the veins flowing through the marble and make them sparkle like the thousand stars in the firmament.";

        Brief = "You are in the solar, light, sparkles, etc.";

        Move(GetObject("palantine/soranus"), this);

        OpenLink(Direction.WEST, "palantine\\antechamber");
        OpenLink(Direction.EAST, "palantine\\platos_closet");

        Perform<MudObject, solar>("describe")
                .Do((viewer, item) =>
                {
                    var auto = Core.ExecutingCommand.ValueOrDefault("AUTO", false);

                    if (item.TimesViewed > 0 && auto)
                        MudObject.SendMessage(viewer, item.Brief);
                    else
                        MudObject.SendMessage(viewer, item.Long);

                    item.TimesViewed += 1;
                    return PerformResult.Stop;
                }).Name("Choose brief or long description rule.");

    }
}