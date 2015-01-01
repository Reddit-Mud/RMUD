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
        Links = 2,
        Scenery = 4,

        In = 8,
        On = 16,
        Under = 32,
        Behind = 64,

        Held = 128,
        Worn = 256,
    }
}
