public class lighthouse_stairs : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Saint Reyel Lighthouse, Stairwell";
		Long = "The iron-cast stairs are half rusted away, spiraling up and up into a darkness cascading back down, leaving only a dim light " +
			"sneaking in from the lobby below. Partway up, the hole where a window would normally be is instead covered by planks of wood, " +
			"nailed crudely to the walls. Glass shards cover the stairs. The shadow and silence combine with a heavy-feeling in the air; the " +
			"eerie ambiance almost pushing visitors out as quickly as possible. \n\nThe balcony above is blocked by an iron hatch, firmly seated " +
			"at the top of the stairwell.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.UP, "devrw-demo/lighthouse_balcony");
		OpenLink(RMUD.Direction.DOWN, "devrw-demo/lighthouse");

	}
}
