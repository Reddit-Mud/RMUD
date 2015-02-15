using RMUD;

namespace Space
{
    public class Wrench : MudObject
    {
        public Wrench()
        {
            Long = "It's a big heavy orange wrench.";
            SimpleName("wrench", "spanner");
            SetProperty("heavy", true);
        }
    }   
}