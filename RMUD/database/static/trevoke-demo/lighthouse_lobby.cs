public class lighthouse_lobby : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Lighthouse Lobby";

                Long = "Impossibly, there is a single electric lightbulb illuminating the room. The stark light shows cobwebs, a spiraling staircase, and a hanging painting."

                AddScenery("The painting is of a man posing in the old-fashioned way. He is grinning with all his teeth and his eyes are wide open. The effect is a little disturbing.", "painting", "hanging painting");

                AddScenery("Cobwebs. ... Okay, fine. There are also spiders and flies that got caught. And they look like no one's been here to clean them in a while. Then again, this is an abandoned lighthouse. Savvy?", "cobwebs");

                OpenLink(RMUD.Direction.WEST, "trevoke-demo/shoreline", RMUD.Mud.GetObject("trevoke-demo/lighthouse_door") as RMUD.Door);
                OpenLink(RMUD.Direction.UP, "trevoke-demo/lighthouse_stairway");
        }
}
