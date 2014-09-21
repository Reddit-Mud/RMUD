public class shack : RMUD.Room
{
	override public void Initialize()
	{
		Short = "Small Shack";
		Long = "The proud, sturdy walls display battle scars from the elements. There are a few [fishing poles] leaning against one corner of the shack, and a [lobster trap] in another. A small table is near the trap, showing evidence of the day's catch having been cleaned upon it. A slight aroma of fish hangs in the air. There is a boat docked outside to the west.";

		OpenLink(RMUD.Direction.NORTH, "piggy-demo/shoreline");
		OpenLink(RMUD.Direction.WEST, "piggy-demo/boat_deck");

		AddScenery("They look old, but durable.", "fishing", "poles");
		AddScenery("The frame seems to be bent. You wonder if it is usable.", "lobster", "trap");
	}
}
