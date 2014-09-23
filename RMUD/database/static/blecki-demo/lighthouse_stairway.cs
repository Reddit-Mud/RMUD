public class lighthouse_stairway : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Stairway";
        Long = "A series of cast iron treads climb through the center of the lighthouse. The soles of countless boots and the souls of countless passers have buffed them to just the faintest of a gleam. Dust stands still in the air, illuminated by beams piercing the dull trapped eternity inside. The windows are always placed too high to see out, no matter how many stairs you climb.";

		OpenLink(RMUD.Direction.WEST, "blecki-demo/lighthouse_balcony");
		OpenLink(RMUD.Direction.DOWN, "blecki-demo/lighthouse_lobby");
	}
}
