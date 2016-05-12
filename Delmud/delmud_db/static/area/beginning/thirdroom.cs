public class thirdroom : MudObject
{
    public override void Initialize()
    {
        Room(RoomType.Interior);

        SetProperty("short", "The Second Room");
        SetProperty("ambient light", LightingLevel.Bright);
        OpenLink(Direction.SOUTH, "area/beginning/secondroom");

    }
}