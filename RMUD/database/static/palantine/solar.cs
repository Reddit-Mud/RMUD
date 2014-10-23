public class solar : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Exterior;
        Short = "Palantine Villa - Solar";

        RMUD.MudObject.Move(RMUD.Mud.GetObject("palantine/soranus"), this);

        OpenLink(RMUD.Direction.WEST, "palantine\\antechamber");
	}
}