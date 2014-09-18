public class lighthouse_stairway : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Stairway";

		OpenLink(RMUD.Direction.WEST, "blecki-demo/lighthouse_balcony");
		OpenLink(RMUD.Direction.DOWN, "blecki-demo/lighthouse_lobby");
	}
}
