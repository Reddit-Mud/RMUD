using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    [Flags]
    public enum RelativeLocations
    {
        None = 0,
        Default = 0,

        Contents = 1, //As of a room

        In = 8,
        On = 16,
        Under = 32,
        Behind = 64,

        Held = 128,
        Worn = 256,
    }

    public static class Relloc
    {
        public static String GetRelativeLocationName(RelativeLocations Location)
        {
            if ((Location & RelativeLocations.On) == RelativeLocations.On)
                return "on";
            else if ((Location & RelativeLocations.In) == RelativeLocations.In)
                return "in";
            else if ((Location & RelativeLocations.Under) == RelativeLocations.Under)
                return "under";
            else if ((Location & RelativeLocations.Behind) == RelativeLocations.Behind)
                return "behind";
            else
                return "relloc";
        }
    }
}
