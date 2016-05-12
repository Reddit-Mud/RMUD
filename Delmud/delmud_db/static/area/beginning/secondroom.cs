public class secondroom : MudObject
{
    public override void Initialize()
    {
        Room(RoomType.Interior);

        SetProperty("short", "The Second Room");
        SetProperty("ambient light", LightingLevel.Bright);
        OpenLink(Direction.SOUTH, "start");
        OpenLink(Direction.NORTH, "area/beginning/thirdroom");

    }
}