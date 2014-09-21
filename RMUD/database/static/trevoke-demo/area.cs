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
                Short = "Trevoke's demo area";
                Long = "Go IN to visit Trevoke's demo area.";

                OpenLink(RMUD.Direction.IN, "trevoke-demo/shoreline");
                OpenLink(RMUD.Direction.NORTH, "dummy");
        }

}

/*
We're going to build a small section of land just outside the city. A stretch of the beach, a fishing shack along it, a boat tied to the shore, and a lighthouse overlooking it all. This will cover several indoor and outdoor areas, as well as areas that offer you more possible details to work with, and others that may be a bit more barren.

The descriptions, at this time, should be static, and all objects in the room should have only two properties: a name and a description, unless @Blecki has more functionality prepared -- if so, we could try having a key and a locked door to the lighthouse. Otherwise, let's keep it simple.

ROOMS

Shoreline
Fishing Shack
Fishing Boat Deck
Fishing Boat Cabin
Rocky Bluff
Lighthouse Lobby
Lighthouse Stairwell
Lighthouse Balcony
Feel free to take creative liberties with the names.
*/
