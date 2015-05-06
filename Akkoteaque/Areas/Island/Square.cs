using RMUD;

namespace Akko.Areas.Island
{

    public class Square : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Town Square";

            OpenLink(Direction.WEST, "Start");
        }
    }   
}