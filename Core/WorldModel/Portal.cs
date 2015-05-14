using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    /// <summary>
    /// Contains helper functions for the implementation of portals. (For example, doors.)
    /// </summary>
    public static class Portal
    {
        /// <summary>
        /// Given a portal, find the opposite side. Portals are used to connect two rooms. The opposite side of 
        /// the portal is assumed to be the portal in the linked room that faces the opposite direction. For example,
        /// if portal A is in room 1, faces west, and is linked to room 2, the opposite side would be the portal
        /// in room 2 that faces east. It does not actually check to see if the opposite side it finds is linked
        /// to the correct room.
        /// </summary>
        /// <param name="Portal"></param>
        /// <returns></returns>
        public static MudObject FindOppositeSide(MudObject Portal)
        {
            // Every object added to a room as a portal will be given the 'portal?' property, with a value of true.
            if (Portal.GetPropertyOrDefault<bool>("portal?", false) == false) return null; // Not a portal.

            var destination = MudObject.GetObject(Portal.GetProperty<String>("link destination")) as Room;
            if (destination == null) return null; // Link is malformed in some way.
            
            var direction = Portal.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE);
            var oppositeDirection = Link.Opposite(direction);
            var mirrorLink = destination.EnumerateObjects().FirstOrDefault(p =>
                p.GetBooleanProperty("portal?") && p.GetPropertyOrDefault<Direction>("link direction", Direction.NOWHERE) == oppositeDirection);
            return mirrorLink;
        }
    }
}
