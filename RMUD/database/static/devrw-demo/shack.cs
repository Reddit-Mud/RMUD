public class shack : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Shack, Main Room";
		Long = "Driftwood walls and a palm leaf ceiling are all the protection this shack has from the elements. How it still stands, " +
			"or whether it is still visited, is a question without answer. Ambient light pours in from every crevice within the walls. " +
			"Atop a series of mismatched wooden planks sits a piece of black tarp, stained with dirt, providing an uncomfortable but working " +
			"floor. Tackle boxes organized by some unknown metric are stacked everywhere, complementing the dozens of fishing poles, each unique," +
			" hanging on every wall. A single opening provides a view of the boat tethered to the shore just outside. A thin stick hanging from " +
			"the ceiling holds a small ring, a single key tied around it with string.\n\nThe doorway, covered by loosely hanging cloth, leads back " +
			"outside.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.OUT, "devrw-demo/shoreline");

	}
}
