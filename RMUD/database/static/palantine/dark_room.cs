public class dark_room : RMUD.Room
{
	public override void Initialize()
	{
        RoomType = RMUD.RoomType.Interior;
        Short = "Palantine Villa - Soul Chamber";
        Long = "It does not matter how bright a light you carry, it cannot banish the shadows from your soul.";

        OpenLink(RMUD.Direction.WEST, "palantine\\disambig", GetObject("palantine\\disambig_red_door@outside"));
	}
}
