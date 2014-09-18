public class deck : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Boat Deck";

		OpenLink(RMUD.Direction.EAST, "blecki-demo/shoreline");
		OpenLink(RMUD.Direction.WEST, "blecki-demo/cabin");
	}
}
