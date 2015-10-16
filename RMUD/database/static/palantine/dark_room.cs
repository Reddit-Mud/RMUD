public class dark_room : Room
{
	public override void Initialize()
	{
        RoomType = RoomType.Interior;
        Short = "Palantine Villa - Soul Chamber";
        Long = "It does not matter how bright a light you carry, it cannot banish the shadows from your soul.";

        OpenLink(Direction.WEST, "palantine\\disambig", GetObject("palantine\\disambig_red_door@outside"));
	}
}
