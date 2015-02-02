using RMUD;

namespace Space
{
    public class Common : Room
    {
        public override void Initialize()
        {
            Long = "Now I'm in the common room. Everything is floating around.";
            QuickDescription = "I'm in the common room again.";

            OpenLink(Direction.NORTH, "Cargo", GetObject("CommonCargoHatch"));
        }
    }   
}