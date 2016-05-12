using RMUD;

namespace Minimum
{
    public class Player : RMUD.MudObject
    {
        public override void Initialize()
        {
            Actor();

            SetProperty("short", "you");
        }
    }
}
