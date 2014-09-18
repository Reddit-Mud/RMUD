public class bluff : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Rocky Bluff";

		OpenLink(RMUD.Direction.SOUTH, "blecki-demo/shoreline");
	}
}
