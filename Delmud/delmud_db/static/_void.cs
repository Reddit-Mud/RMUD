public class _void : MudObject
{
	public override void Initialize()
	{
        Room(RoomType.Interior);

        SetProperty("short", "The Endless Void");
        SetProperty("ambient light", LightingLevel.Bright);

        OpenLink(RMUD.Direction.NORTH, "start");
    }
}
