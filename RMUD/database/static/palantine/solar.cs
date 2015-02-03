public class solar : RMUD.Room
{
    int TimesViewed = 0;
    string Brief;

    public override void Initialize()
    {
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Solar";

        Long = "You are in a gorgeous solar filled with white marble and glowing sun beams. Massive windows above let in copious amounts of light that pick out flecks of minerals in the veins flowing through the marble and make them sparkle like the thousand stars in the firmament.";

        Brief = "You are in the solar, light, sparkles, etc.";

        Move(GetObject("palantine/soranus"), this);

        OpenLink(RMUD.Direction.WEST, "palantine\\antechamber");
        OpenLink(RMUD.Direction.EAST, "palantine\\platos_closet");

        Perform<RMUD.MudObject, solar>("describe")
                .Do((viewer, item) =>
                {
                    var auto = RMUD.Core.ExecutingCommand.ValueOrDefault("AUTO", false);

                    if (item.TimesViewed > 0 && auto)
                        RMUD.MudObject.SendMessage(viewer, item.Brief);
                    else
                        RMUD.MudObject.SendMessage(viewer, item.Long);

                    item.TimesViewed += 1;
                    return RMUD.PerformResult.Stop;
                }).Name("Choose brief or long description rule.");

    }
}