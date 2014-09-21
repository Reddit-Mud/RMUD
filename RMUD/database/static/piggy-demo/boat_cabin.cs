public class boat_cabin : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Cabin on the Boat";
		Long = "The windows have a slight film on them, perhaps from smoke. There's a fish finder with a cracked screen mounted on the throttle housing. A compass bobs dispassionately above the rudder wheel, pointing to North at your back. The [keys] dangle in the ignition, showing off a small photo keychain with a woman's [picture] in it. There is a marine band [radio] mounted to the roof, keeping watch over a small folder of [maps].";

		OpenLink(RMUD.Direction.NORTH, "piggy-demo/boat_deck");

		AddScenery("they seem to be stuck in the ignition", "keys");
		AddScenery("the woman looks to be in her mid-40's, wearing a conservative green dress.", "picture");
		AddScenery("you flip the power on, but only hear the whisper of static", "radio");
		AddScenery("there is a map of the island, and some depth charts of the the surrounding ocean", "maps");
	}
}

