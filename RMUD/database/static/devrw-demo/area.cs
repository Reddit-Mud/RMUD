public class area : RMUD.Room
{
	public override void Initialize()
	{
		Short = "DevRW's Demo Map";
		Long = "A portal stands here, its inviting light shimmering brightly. Whispers echo ominously in the air...\n" +
			"\"Go in, my child. Go in.";

		// Object descriptions here.
		AddScenery("Marble pillars centered around an altar-esque table, indentations notched into the sides where light seems to" +
			"stream from endlessly.", "portal");
		AddScenery("Endless amounts of colors swirl into a white, hovering sphere.", "light");

		OpenLink(RMUD.Direction.IN, "devrw-demo/shoreline");
		OpenLink(RMUD.Direction.WEST, "dummy");
	}
}
