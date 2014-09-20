public class lighthouse_lobby : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Lobby";

		OpenLink(RMUD.Direction.WEST, "blecki-demo/shoreline", RMUD.Mud.GetObject("blecki-demo/lighthouse_door") as RMUD.Door);
		OpenLink(RMUD.Direction.UP, "blecki-demo/lighthouse_stairway");
	}
}
