public class shoreline : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Shoreline";


		OpenLink(RMUD.Direction.OUT, "blecki-demo/area");

		OpenLink(RMUD.Direction.WEST, "blecki-demo/deck");
		OpenLink(RMUD.Direction.NORTH, "blecki-demo/bluff");
		OpenLink(RMUD.Direction.EAST, "blecki-demo/lighthouse_lobby");
		OpenLink(RMUD.Direction.SOUTH, "blecki-demo/shack");
	}
}
