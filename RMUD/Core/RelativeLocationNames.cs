using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class MudObject
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
