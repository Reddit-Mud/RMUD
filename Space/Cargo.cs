using RMUD;

namespace Space
{
    public class Cargo : Room
    {
        public override void Initialize()
        {
            Long = "Now I'm in the cargo bay. Everything is floating around.";
            QuickDescription = "I'm in the cargo bay again.";

            OpenLink(Direction.EAST, "Start");
        }
    }   
}