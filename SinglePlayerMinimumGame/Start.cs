using RMUD;

namespace Minimum
{

    public class Start : RMUD.MudObject
    {
        public override void Initialize()
        {
            Room(RoomType.Exterior);
            SetProperty("Short", "Start Room");
            SetProperty("Long", "This is a game with the minimum possible objects.");
        }
    }   
}