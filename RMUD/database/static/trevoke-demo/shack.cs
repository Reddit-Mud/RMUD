public class shack : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Fishing Shack";

                Long = "The light coming in from the outside illuminates the back wall, revealing many tools and a workbench. There is a cleaver hacked into the workbench and, next to the cleaver, a key. The shadows reveal a darker shadow on the ground, like a large puddle of blackness. ";

                AddScenery("This is an unassuming workbench. It looks like it has been well-worked. At a guess, one would think it was used both for gutting, cleaning, and preparing fish, as well as for odds and ends of woodworking.", "workbench");

                AddScenery("This cleaver has been stuck into the workbench just the way people do in movies when they want to look cool. It's terrible for the cleaver. Since there's no one around, it's possible that no one cared.", "cleaver");

                AddScenery("The shadow seems to elude attempts to be looked at. It trickles, pools, flows its way around your focus. It's always there, but never quite where you look.", "shadow");

                OpenLink(RMUD.Direction.NORTH, "trevoke-demo/shoreline");

                RMUD.Thing.Move(RMUD.Mud.GetObject("trevoke-demo/lighthouse_key") as RMUD.Thing, this);
        }
}
