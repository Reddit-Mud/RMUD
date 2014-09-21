public class cabin : RMUD.Room
{
        public override void Initialize()
        {
                Short = "Fishing Boat Cabin";

                Long = "The cabin is as spartan as the deck is. There is an unmade bed, a table with a quill, an inkpot, an open book and a candle, and a small, open closet.";
                AddScenery("The bed's sheets look as though they may have been kicked away from the bed's occupant.", "bed");

                AddScenery("The table is a standing desk. Odd in a boat, but all is possible. The open book beckons.", "table");

                AddScenery("The book, from what you can see, is a mix of travel diary and... Something darker. There are ominous symbols and an inscription in what might be Latin. The inscription seems to have been interrupted mid-word.", "book", "open book");

                AddScenery("The closet has three hangers and a pair of waterproof boots.", "closet");

                AddScenery("They are hangers. For hanging up things.", "hangers");

                AddScenery("They are boots. For when you have to step in water and you want your feet to stay dry.", "boots");

                OpenLink(RMUD.Direction.EAST, "trevoke-demo/deck");

        }
}
