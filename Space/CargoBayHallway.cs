using RMUD;

namespace Space
{
    public class CargoBayHallway : Room
    {
        public override void Initialize()
        {
            AirLevel = Space.AirLevel.Vacuum;

            Long = "Now I'm in the common room. Everything is floating around.";
            BriefDescription = "I'm in the common room again.";

            OpenLink(Direction.NORTH, "Cargo", GetObject("Hatch@CargoHallway"));
        }
    }   
}