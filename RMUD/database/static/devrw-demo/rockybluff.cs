public class rockybluff : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Ingradia, Rocky Bluff";
		Long = "Long since traveled, flora has grown with free reign here. The plant-like arms reaching out onto the pathway at shoulder-height " +
			"try to hide the doorway leading into the lighthouse that towers just above. Just above the tops of some plants, the bluff opens up " +
			"onto a long, empty stone surface. The ocean waves crash against it, splashing a fine mist onto the side of the building. Above, gulls" +
			"dance wildly around the unlit beacon, likely claiming ownership of the tower now.\n\nThe overgrown trail winds down to the south-east, " +
			"back to the shoreline.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.IN, "devrw-demo/lighthouse");
		OpenLink(RMUD.Direction.SOUTHEAST, "devrw-demo/shoreline");

	}
}
