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

            AddScenery(GetObject("CargoRoomGrate"));
            Move(GetObject("CargoRoomBoxes"), this);

            OpenLink(Direction.EAST, "Start");
            OpenLink(Direction.SOUTH, "CargoBayHallway", GetObject("Hatch@CargoSouth"));

            var locker = new Locker();
            Move(locker, this);
            Move(GetObject("Wrench"), locker);
            Move(GetObject("DuctTape"), locker);
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
                    return SharpRuleEngine.CheckResult.Disallow;
                });
        }
    }

    public class CargoRoomGrate : MudObject
    {
        public override void Initialize()
        {
            SimpleName("grate");
            Long = "There is a grate in the wall behind one of the cargo boxes.";
        }
    }
}