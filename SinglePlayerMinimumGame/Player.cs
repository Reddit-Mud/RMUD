using RMUD;

namespace Minimum
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            SetProperty("Short", "you");
        }
    }
}
