public class lighthouse : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Saint Reyel Lighthouse, Lobby";
		Long = "The sudden, overwhelming silence is a stark contrast to outside. Aside from the natural light creeping in from outside, the lobby " +
			"sits in darkness. Dirty pieces of glass lay sprinkled on the granite floor, probably fallen from above. Cobwebs stretch along every " +
			"other object in the room, dead moths and other insects every few feet, seemingly untouched. A desk, long fallen into disrepair, lays " +
			"on its side against one of the walls. A few books are scattered along the floor, completely weathered down and unreadable. The door " +
			"leading outside creaks sharply. The atmosphere is off-putting.\n\nAlong the walls, stairs wind upward into an unrelenting darkness.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.OUT, "devrw-demo/rockybluff");
		OpenLink(RMUD.Direction.UP, "devrw-demo/lighthouse_stairs");

	}
}
