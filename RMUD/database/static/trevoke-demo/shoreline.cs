public class shoreline : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Shoreline";
                Long = "The beach's eternal changing state cleans any trace of a path. Even your tracks seem to disappear too fast. The abandoned lighthouse casts its shadow behind you, darker than any shadow has a right to be in such a desolate place. Nearby, a small shack seems to be waiting to be noticed, and a boat, tied to the shore, seems to be fighting the waves and the ropes in an effort to earn its freedom.";

                AddScenery("The lighthouse casts its shadow upon the beach in the direction of the bluff. It looks like it hasn't been occupied in years. The front door is ajar, and its wood is showing signs of rot. Some of the windows are boarded shut, and there is glass at the foot of the building, no doubt coming from the other windows. You almost *want* to see cobwebs, but there are none.", "lighthouse", "light", "house");

                AddScenery("If this shack were a person, you probably wouldn't give it the time of day. It's only standing because falling was too much effort. It is made of tall planks of wood nailed together. From here, two openings can be seen, one no doubt meant to be a door, and one a window.", "shack");

                AddScenery("This boat looks to be low on the water, as though it had a heavy cargo. It is a single-person boat, the kind that could be taken out to sea for a few days by a man longing for loneliness and the desire to measure his worth against inexorable foes.", "boat");

                OpenLink(RMUD.Direction.OUT, "trevoke-demo/area");

                OpenLink(RMUD.Direction.WEST, "trevoke-demo/deck");
                OpenLink(RMUD.Direction.NORTH, "trevoke-demo/bluff");
                OpenLink(RMUD.Direction.EAST, "trevoke-demo/lighthouse_lobby", RMUD.Mud.GetObject("trevoke-demo/lighthouse_door") as RMUD.Door);
                OpenLink(RMUD.Direction.SOUTH, "trevoke-demo/shack");
        }
}
