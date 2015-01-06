using RMUD;

namespace SinglePlayer.Database
{

    public class Bar : RMUD.Room
    {
        
        public override void Initialize()
        {
            RoomType = RMUD.RoomType.Interior;
            AmbientLighting = LightingLevel.Dark;
            OpenLink(Direction.NORTH, "Foyer");
        }
    }

   
}