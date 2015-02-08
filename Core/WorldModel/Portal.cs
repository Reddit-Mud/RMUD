using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class Portal
    {
        public static MudObject FindOppositeSide(MudObject Portal)
        {
            if (Portal.GetPropertyOrDefault<bool>("portal?", false) == false) return null; //Not a portal.
            var destination = MudObject.GetObject(Portal.GetProperty<String>("link destination")) as Room;
            if (destination == null) return null; //Link is malformed in some way.
            var direction = Portal.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE);
            var oppositeDirection = Link.Opposite(direction);
            var mirrorLink = destination.EnumerateObjects(RelativeLocations.Links).FirstOrDefault(p =>
                p.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE) == oppositeDirection);
            return mirrorLink;
        }
    }
}
