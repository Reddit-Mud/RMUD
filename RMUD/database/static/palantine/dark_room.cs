public class dark_room : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Interior;	

        OpenLink(RMUD.Direction.WEST, "palantine\\disambig", RMUD.Mud.GetObject("palantine\\disambig_red_door"));
	}
}
