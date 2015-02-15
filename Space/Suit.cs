using RMUD;

namespace Space
{
    public class Suit : Container
    {
        public Suit() : base(RelativeLocations.On, RelativeLocations.On)
        {
            Long = "This is my space suit.";
            SimpleName("suit", "space", "spacesuit");
        }
    }   
}