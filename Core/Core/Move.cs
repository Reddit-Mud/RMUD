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
        /// <summary>
        /// Move Object to Destination.
        /// </summary>
        /// <param name="Object">Object to move.</param>
        /// <param name="Destination">Destination to move too.</param>
        /// <param name="Location">The relative location within destination to move too.</param>
        public static void Move(MudObject Object, MudObject Destination, RelativeLocations Location = RelativeLocations.Default)
        {
            if (Object == null) return;

            if (Object.Location != null && Object.Location is Container) (Object.Location as Container).Remove(Object);

            if (Destination is Container) 
            {
                (Destination as Container).Add(Object, Location);
                Object.Location = Destination;
            }
            else
                Object.Location = null;
        }
    }
}
