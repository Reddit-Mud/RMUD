public class bluff : RMUD.Room
{
	override public void Initialize()
	{
		Short = "Rocky Bluff";
		Long = "You follow the storm-worm fence to rougher terrain. The rocks here are slick with sea spray, and offer the quiet threat of hard edges. A few birds wait patiently for fish to get dashed against the rocks. A dark stain mars the surface of some rocks below the bluff, but it's hard to tell what may have left it there. You notice a small [silver locket] and [cigarette butts] near a flat rock. It looks like someone may have been sitting there.";

		OpenLink(RMUD.Direction.WEST, "piggy-demo/shoreline");

		AddScenery("it's covered in dirt, but there is a photo inside of an older woman in a green dress", "silver", "locket");
		AddScenery("there are many, but only one has lipstick on the filter", "cigarette", "butts", "fags");
	}
}

