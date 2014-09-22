public class boat_deck : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Boat, Deck";
		Long = "Boxes and barrels cover the wooden deck, each filled with a variety of sea-faring equipment. Coils of hemp rope and " +
			"netting, stacks of empty buckets, and a life saver lie in a disorganized mess at the bow. The mast towers far above, " +
			"rigging grabbing out in all directions like a web with the sails waiting to be drawn. Toward the stern, a large " +
			"enclosure with glass windows houses the vessels centered helm, its door falling off the hinges.\n\nA ramp is drawn on the port" +
			"side of the deck, leading back onto shore.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.IN, "devrw-demo/boat_helm");
		OpenLink(RMUD.Direction.WEST, "devrw-demo/shoreline");
		
	}
}
