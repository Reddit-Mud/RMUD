using RMUD;

namespace Space
{
    public class Start : Room
    {
        public override void Initialize()
        {
            Long = "I think I'm in some kind of closet. It's not very big and there isn't much light.";
            QuickDescription = "I'm in the closet again.";
            Short = "utility closet";

            OpenLink(Direction.WEST, "Cargo");
        }
    }   
}