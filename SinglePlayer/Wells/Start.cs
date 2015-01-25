using RMUD;

namespace Wells
{

    public class Start : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Cistern Inn - Mud Room";
            Long = "This is the muddy mud room of the Cistern Inn. It sits between the slightly less muddy common room and the exceptionally muddy front porch. Loose thresh scattered about the floor does a very poor job of soaking up the filth.";

            Move(GetObject("Thrad"), this);
        }
    }   
}