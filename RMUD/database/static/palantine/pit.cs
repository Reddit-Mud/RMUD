public class pit : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Pit";

        OpenLink(RMUD.Direction.NORTH, "palantine/antechamber");
	}
}