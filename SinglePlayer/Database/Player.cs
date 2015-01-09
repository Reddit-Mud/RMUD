using RMUD;

namespace SinglePlayer.Database
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            Short = "you";

            Move(GetObject("Cloak"), this, RelativeLocations.Worn);
        }
    }
}
