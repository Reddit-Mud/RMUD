public class boat_helm : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Boat, Helm";
		Long = "Comfortably away from the weather, the polished, aluminum wheel stands front and center. Windows surround the helm, " +
			"providing a full view of the ocean at sea. Several shards of glass are strewn among the floor from the shattered ceiling " +
			"light. Where there are no windows, maps of the world covered in markings are tacked onto the wooden walls alongside other " +
			"papers. \n\nOff to the side, a staircase leads below deck. Back outside, salty air pours into the small cabin.";

		// Object descriptions here.
		AddScenery("");


		// Exits.
		OpenLink(RMUD.Direction.OUT, "devrw-demo/area");
		OpenLink(RMUD.Direction.DOWN, "devrw-demo/boat_cabin");
		
	}
}
