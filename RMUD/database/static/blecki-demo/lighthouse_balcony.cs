public class lighthouse_balcony : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Balcony";

		OpenLink(RMUD.Direction.EAST, "blecki-demo/lighthouse_stairway");
	}
}
