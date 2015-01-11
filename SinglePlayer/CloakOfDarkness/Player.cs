using RMUD;

namespace CloakOfDarkness
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
