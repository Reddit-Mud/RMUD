public class lobby : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Lighthouse Lobby";
		Long = "Cabernet wood trim accent the walls and [hearth] of the Lighthouse Lobby, matching the picture frames featuring moody scenes of proud sailing ships that seem to beckon you to a life at sea. Nestled against one side of the hearth is a small oak desk that hosts piles of [maps] and charts, and a rather imposing marine band [radio], accompanied by a puffy looking red leather chair. A single large window sits open to the west, letting in the cool ocean air and sounds of the sea. There is a landing for a spiral staircase in the middle of the room.";

		OpenLink(RMUD.Direction.SOUTH, "piggy-demo/shoreline");
		OpenLink(RMUD.Direction.UP, "piggy-demo/stairwell");

		AddScenery("it's certainly large enough to heat this small room, but looks unused for some time.", "hearth");
		AddScenery("there is a map of the island and a few depth and current charts", "maps");
		AddScenery("you hear a droning tin voice repeating weather data", "radio");
	}
}

