using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class MatchContext
    {
        public Actor ExecutingActor;

        private List<MudObject> CachedObjectsInScope = null;
        public List<MudObject> ObjectsInScope
        {
            get
            {
                if (CachedObjectsInScope != null) return CachedObjectsInScope;
                CachedObjectsInScope = new List<MudObject>();

                var location = ExecutingActor.Location as Room;
                if (location != null)
                    location.EnumerateObjects(RelativeLocations.Room, EnumerateCallback(CachedObjectsInScope));

                return CachedObjectsInScope;
            }
        }

        private static Func<MudObject, RelativeLocations, EnumerateObjectsControl> EnumerateCallback(List<MudObject> Into)
        {
            return (o, l) =>
                {
                    Into.Add(o);

                    var container = o as Container;

                    if (container != null)
                    {
                        EnumerateContainer(container, RelativeLocations.On, Into);
                        if (Mud.IsOpen(container as MudObject))
                            EnumerateContainer(container, RelativeLocations.In, Into);
                        EnumerateContainer(container, RelativeLocations.Behind, Into);
                        EnumerateContainer(container, RelativeLocations.Under, Into);

                        if (container is Actor) 
                            EnumerateContainer(container, RelativeLocations.Player, Into);
                    }

                    return EnumerateObjectsControl.Continue;
                };
        }

        private static void EnumerateContainer(Container Container, RelativeLocations Location, List<MudObject> Into)
        {
            Container.EnumerateObjects(Location, (o, l) =>
                {
                        Into.Add(o);
                    return EnumerateObjectsControl.Continue;
                });
        }
    }
}
