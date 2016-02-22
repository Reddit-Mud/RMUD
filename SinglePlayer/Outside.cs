using RMUD;

namespace CloakOfDarkness
{
    public class Outside : RMUD.MudObject
    {
        public override void Initialize()
        {
            Room(RoomType.Exterior);

            SetProperty("short", "Outside the Opera House");
        }
    }
}