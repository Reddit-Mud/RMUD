public class lighthouse_lobby : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Lobby";
        Long = "A ponderous emptyness hangs about the bottom of this lighthouse. The walls are plain gray cinderblock, the floor is a slab of greenish concrete, and quite literally nothing surrounds the base of the spiralling staircase that stabs through the ceiling. A few thin windows full of thick glass don't let in enough light.";

		OpenLink(RMUD.Direction.WEST, "blecki-demo/shoreline", RMUD.Mud.GetObject("blecki-demo/lighthouse_door"));
		OpenLink(RMUD.Direction.UP, "blecki-demo/lighthouse_stairway");
	}
}
