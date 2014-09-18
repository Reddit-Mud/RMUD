public class shack : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Shack";

		OpenLink(RMUD.Direction.NORTH, "blecki-demo/shoreline");
	}
}
