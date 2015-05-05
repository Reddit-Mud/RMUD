public class garden : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Garden";

        Move(GetObject("palantine/wolf"), this);

        OpenLink(RMUD.Direction.EAST, "palantine\\antechamber");
        OpenLink(RMUD.Direction.DOWN, "palantine\\cave");
    }
}