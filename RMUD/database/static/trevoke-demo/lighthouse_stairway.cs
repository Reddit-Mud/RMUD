public class lighthouse_stairway : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Lighthouse Stairway";

                Long = "The wooden spiraling staircase has a few broken steps, showing darkness below, and the rest in perfect condition. The windows that haven't been boarded up are all broken as though from the inside. A soft breeze flows around you. The light is uneven, but it is enough to find one's footing";

                OpenLink(RMUD.Direction.WEST, "trevoke-demo/lighthouse_balcony");
                OpenLink(RMUD.Direction.DOWN, "trevoke-demo/lighthouse_lobby");
        }
}
