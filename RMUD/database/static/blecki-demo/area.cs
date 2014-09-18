using System.Text;

public class area : RMUD.Room
{
	/*                            R
    S Shoreline				C--D--S--L
    F Fishing Shack               |  ^I--A
    D Fishing Boat Deck           F
    C Fishing Boat Cabin
    R Rocky Bluff
    L Lighthouse Lobby
    I Lighthouse Stairwell
    A Lighthouse Balcony
	*/

	public override void Initialize()
	{
		Short = "Blecki's demo area";
		Long = "Go IN to visit Blecki's demo area.";

		OpenLink(RMUD.Direction.IN, "blecki-demo/shoreline");
		OpenLink(RMUD.Direction.NORTH, "dummy");
	}

}
