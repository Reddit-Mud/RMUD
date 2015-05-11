using RMUD;

namespace Akko.Areas.Lighthouse
{
    public static class LighthouseVar
    {
        public static bool Powered = true;
    }

    public class Lamp : RMUD.Scenery
    {
        public override void Initialize()
        {
            SimpleName("lamp");

            this.ValueLightingLevel().Do((o) => LighthouseVar.Powered ? LightingLevel.Bright : LightingLevel.Dark);
        }
    }

    public class Base : RMUD.Room
    {
        public override void Initialize()
        {
            RoomType = RMUD.RoomType.Interior;
            Short = "Base of the Lighthouse";

            OpenLink(Direction.EAST, "LH-CON-INN");
            OpenLink(Direction.NORTH, "Areas.Lighthouse.StorageRoom", GetObject("Areas.Lighthouse.StorageRoomDoor@A"));
            OpenLink(Direction.UP, "Areas.Lighthouse.LowerStair");

            AddScenery(InitializeObject(new Lamp()));
        }
    }

    public class LowerStair : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Lower Stairway";

            OpenLink(Direction.DOWN, "Areas.Lighthouse.Base");
            OpenLink(Direction.UP, "Areas.Lighthouse.UpperStair");
        }
    }

    public class UpperStair : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Upper Stairway";

            OpenLink(Direction.DOWN, "Areas.Lighthouse.LowerStair");
            OpenLink(Direction.UP, "Areas.Lighthouse.Control");
        }
    }

    public class Control : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Control Room";

            OpenLink(Direction.DOWN, "Areas.Lighthouse.UpperStair");
            OpenLink(Direction.EAST, "Areas.Lighthouse.Balcony", GetObject("Areas.Lighthouse.Hatch@C"));
        }
    }

    public class Balcony : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Balcony";

            OpenLink(Direction.WEST, "Areas.Lighthouse.Control", GetObject("Areas.Lighthouse.Hatch@D"));
        }
    }

    public class StorageRoomDoor : RMUD.BasicDoor
    {
        public override void Initialize()
        {
            Short = "door";
        }
    }

    public class StorageRoom : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Storage room";

            OpenLink(Direction.SOUTH, "Areas.Lighthouse.Base", GetObject("Areas.Lighthouse.StorageRoomDoor@B"));
            OpenLink(Direction.EAST, "Areas.Lighthouse.Ledge", GetObject("Areas.Lighthouse.Hatch@A"));
        }
    }

    public class Hatch : RMUD.BasicDoor
    {
        public override void Initialize()
        {
            Short = "hatch";
            Nouns.Add("hatch");
        }
    }

    public class Ledge : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Slippery Ledge";

            OpenLink(Direction.WEST, "Areas.Lighthouse.StorageRoom", GetObject("Areas.Lighthouse.Hatch@B"));
            OpenLink(Direction.SOUTH, "Areas.Lighthouse.Alcove");
            OpenLink(Direction.DOWN, "LH-CON-SHORE");
        }
    }

    public class Alcove : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Electrical Alcove";
            OpenLink(Direction.NORTH, "Areas.Lighthouse.Ledge");

        }
    }
}