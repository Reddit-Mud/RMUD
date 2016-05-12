public class cave : Room
{
	public override void Initialize()
	{
        RoomType = RoomType.Exterior;
        Short = "Palantine Villa - Cave";

        OpenLink(Direction.UP, "palantine\\garden");
	}
}