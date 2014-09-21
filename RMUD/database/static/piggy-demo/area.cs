public class area : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Piggy Bank's demo area";
		Long = "Go IN to visit Piggy Bank's demo area.";

		OpenLink(RMUD.Direction.IN, "piggy-demo/shoreline");
		OpenLink(RMUD.Direction.EAST, "dummy");
	}
}
