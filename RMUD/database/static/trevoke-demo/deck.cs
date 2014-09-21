public class deck : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Fishing Boat Deck";

                Long = "The boat's sails are down. On the port side there are three fishing rods lying on the ground. In the back, there is a small engine, an effort to use technology to give man a fighting chance when all else fails. On the starboard side there are two oars of good quality. Stairs lead down to a cabin.";

                AddScenery("These fishing rods seem to be in working order, though they don't seem well-maintained. One of them even has some leftover fish fragments on it. The fish fragments look fresh.", "fishing rods", "rods");

                AddScenery("These oars probably don't come from this boat. They are quite good quality. The wood is lighter than any wood on the boat. One of them has a dark, dried, crusted stain at the end. The stain looks old.", "oars");

                AddScenery("Upon closer examination, scratching and sniffing of the stain, you believe it to be blood.", "stain");

                AddScenery("This is just one of those engines connected to a propeller wheel. It smells of oil, as it should.", "engine");

                OpenLink(RMUD.Direction.EAST, "trevoke-demo/shoreline");
                OpenLink(RMUD.Direction.WEST, "trevoke-demo/cabin");
        }
}
