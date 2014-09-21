public class shoreline : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Ingradia, Along the Eastern Shoreline";
		Long = "Black mud paints splotches on the gravel beach, still wet from the tides recessing.  Chunks of dead seaweed lay heaped " +
		"over the slimy, moss-covered stones that run the length of the shore. Among the rocks, small pools of water have formed, " +
		"housing tiny crabs and other ocean-life. A rope tied around one of the larger stones holds a fishing boat in place on the " +
		"water.\n\nAn old shack stands on the far edge of the shore, just outside the tides grasp. Just beyond it, a small trail that " +
		"has fallen in disuse winds north-west toward a massive lighthouse, set atop a rocky outcropping.";

		// Object descriptions here.
		AddScenery("", "");


		// Exits.
		OpenLink(RMUD.Direction.OUT, "devrw-demo/area");
		OpenLink(RMUD.Direction.EAST, "devrw-demo/boat_deck");
		OpenLink(RMUD.Direction.IN, "devrw-demo/shack");
		OpenLink(RMUD.Direction.NORTHWEST, "devrw-demo/rockybluff");
		
	}
}
