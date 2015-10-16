public class pit : Room
{
	public override void Initialize()
	{
        RoomType = RoomType.Exterior;
        Short = "Palantine Villa - Pit";

        OpenLink(RMUD.Direction.NORTH, "palantine/antechamber");
	}
}