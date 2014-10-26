public class garden : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Garden";

        RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine/wolf"), this);

        OpenLink(RMUD.Direction.EAST, "palantine\\antechamber");
	}
}