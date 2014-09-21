public class boat_deck : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Deck of a Docked Boat";
		Long = "A few old tires cling to the side of the boat, bumping gently against the dock. The deck of the boat is stained with a strange oily substance and emits a high creak as soon as you step on it. Some of the rails are starting to rust, and you notice a [tackle box] strapped to one. There is an old [first aid kid] encased in dingy plastic mounted on a rotting sideboard. The boat floats, but it has seen better days. You wonder if the motor runs. Its cabin sits south, pointing out to sea.";

		OpenLink(RMUD.Direction.EAST, "piggy-demo/shack");
		OpenLink(RMUD.Direction.SOUTH, "piggy-demo/boat_cabin");

		AddScenery("A few lures and hooks. Not much of interest.", "tackle", "box");
		AddScenery("It seems to have never been opened. You wonder if this could come in handy.", "first", "aid", "kit");
	}
}

