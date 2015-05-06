public class cave : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Cave";

        OpenLink(RMUD.Direction.UP, "palantine\\garden");
	}
}