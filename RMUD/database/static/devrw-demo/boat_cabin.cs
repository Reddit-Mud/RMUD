public class boat_cabin : RMUD.Room
{
	public override void Initialize()
	{
		Short = "Fishing Boat, Cabin";
		Long = "Tightly fit below the helm, a strong, musty scent hangs dutifully in the air. The cabin is clearly built for one; a " +
			"single cot resting along the far wall, dressed with a large pillow and blanket. A few pots and utensils lay scattered on " +
			"the counter, near a stove-top. A pair of fixtures on the ceiling have had their bulbs removed, leaving the cabin in " +
			"darkness if not for the windows on either side of the room. \n\nA staircase sits off-centered, leading up to the deck.";

		// Object descriptions here.
		AddScenery("Small; a tiny wooden frame holds together pieces of sand-colored cloth. A blanket covers most of it. ", "cot", "bed");
		AddScenery("Warm and fluffy. It looks fairly clean compared to the rest of the vessel.", "blanket", "pillow");
		AddScenery("Empty but clean, they are all stainless-steel and look fairly new.", "pots");
		AddScenery("Knives, forks, spoons... sporks. They all shine as if they've never been used.", "utensils", "forks", "spoons", "knives");
		AddScenery("Countertops with a faux-marble texture line the walls. A stove is attached to one end, the rest covered in" +
			"utensils and cooking equipment.", "counter");
		AddScenery("Gas-powered, but there is no tank to run it.", "stove");
		AddScenery("The bulbs seem to have been removed from the fixtures, allowing a darkness to creep in " +
			"the room.", "fixture", "light", "lights", "ceiling");
		AddScenery("Polished wooden floors cover the entire floorway, with a small carpet near the cot.", "floor", "ground", "deck");
		AddScenery("Rugged, built for abuse at sea. The soft marks of boots can be seen, recently faded.", "carpet");


		// Exits.
		OpenLink(RMUD.Direction.UP, "devrw-demo/boat_deck");
		
	}
}
