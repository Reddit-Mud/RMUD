using RMUD;

namespace Space
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            Short = "you";
            Move(GetObject("Suit"), this, RelativeLocations.Worn);

            Perform<Actor>("inventory")
                .Do(a =>
                {
                    var suit = GetObject("Suit") as Container;
                    var tapedObjects = suit.GetContents(RelativeLocations.On);
                    if (tapedObjects.Count != 0) 
                    {
                        MudObject.SendMessage(a, "Taped on my space suit is..");
                        foreach (var item in tapedObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List taped items in inventory rule.");
        }
    }
}
