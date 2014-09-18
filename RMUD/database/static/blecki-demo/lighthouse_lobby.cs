public class lighthouse_lobby : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Lobby";

		OpenLink(RMUD.Direction.WEST, "blecki-demo/shoreline");
		OpenLink(RMUD.Direction.UP, "blecki-demo/lighthouse_stairway");
	}
}
