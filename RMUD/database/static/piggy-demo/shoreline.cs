public class shoreline : RMUD.Room
{
	override public void Initialize()
	{
		Short = "Sandy Shoreline";
		Long = "The sand here rough and littered with small stones that dot the areas between clumps of vegitation. A cool breeze feels clean on your skin as the ebb and flow of the sea hypnotizes you with it's rythmic white noise. Part of an old [fence] is here, leading East to a Rocky Bluff.  There is a Lighthouse to the north, and a Fishing Shack to the south.";

		OpenLink(RMUD.Direction.NORTH, "piggy-demo/lobby");
		OpenLink(RMUD.Direction.EAST, "piggy-demo/bluff");
		OpenLink(RMUD.Direction.SOUTH, "piggy-demo/shack");
		OpenLink(RMUD.Direction.OUT, "piggy-demo/area");

		AddScenery("The small wood stakes are held together by wire. This is the only part standing. Looks like the rest has been destroyed by winds.", "fence");
	}
}

