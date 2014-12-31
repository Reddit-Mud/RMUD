using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        public static void Move(MudObject Object, MudObject Destination, RelativeLocations Location = RelativeLocations.Default)
        {
            if (Object.Location != null)
            {
                var container = Object.Location as Container;
                if (container != null)
                    container.Remove(Object);
                Object.Location = null;
            }

            if (Destination != null)
            {
                var destinationContainer = Destination as Container;
                if (destinationContainer != null)
                    destinationContainer.Add(Object, Location);
                Object.Location = Destination;
            }
        }
    }
}
