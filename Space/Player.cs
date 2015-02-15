using RMUD;

namespace Space
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            Short = "you";
            Move(GetObject("Suit"), this, RelativeLocations.Worn);
        }
    }
}
