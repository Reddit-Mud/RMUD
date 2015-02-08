using RMUD;

namespace Space
{
    public class Cargo : Room
    {
        public override void Initialize()
        {
            Long = "Now I'm in the cargo bay. Everything is floating around.";
            BriefDescription = "I'm in the cargo bay again.";
            Short = "cargo bay";

            AddScenery(GetObject("CargoRoomGrate") as Scenery);
            Move(GetObject("CargoRoomBoxes"), this);

            OpenLink(Direction.EAST, "Start");
            OpenLink(Direction.SOUTH, "Common", GetObject("CargoBayHatch@north"));

            var locker = new Locker();
            Move(locker, this);
            Move(new Wrench(), locker);
        }
    }   

    public class CargoRoomBoxes : MudObject
    {
        public override void Initialize()
        {
            Short = "cargo boxes";
            Nouns.Add("BOXES", "CARGO", "CONTAINERS");
            Long = "There are a whole bunch of cargo boxes drifting around.";
            Article = "some";

            Check<MudObject, CargoRoomBoxes>("can take?")
                .Do((actor, boxes) =>
                {
                    SendMessage(actor, "I couldn't carry even one of those.");
                    return CheckResult.Disallow;
                });
        }
    }

    public class CargoRoomGrate : Scenery
    {
        public override void Initialize()
        {
            SimpleName("grate");
            Long = "There is a grate in the wall behind one of the cargo boxes.";
        }
    }
}