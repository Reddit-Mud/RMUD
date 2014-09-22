public class lighthouse_balcony : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Saint Reyel Lighthouse, Balcony";
		Long = "The air moves quickly this high up; a flock of gulls swarming the point ride the gusts freely. Iron-cast rails encircle flooring, " +
			"which goes all around the massive lighting mechanism that once powered this building. A tiny maintenance port, now permanently welded " +
			"off, leads up into beacon room itself. The view is breath-taking -- from this height, the town can be seen at all points and far beyond," +
			" deep into the islands forested locations. The harbor, full of ships, can seen just behind some of the larger buildings." +
			"\n\nThe open hatch exposes the questionable stairwell, leading back down to the lobby.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.DOWN, "devrw-demo/lighthouse_stairs");

	}
}
