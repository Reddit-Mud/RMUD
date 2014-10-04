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

                    if (o is Container)
                    {
                        EnumerateContainer(o, RelativeLocations.On, Into);
                        if (Mud.IsOpen(o))
                            EnumerateContainer(o, RelativeLocations.In, Into);
                        EnumerateContainer(o, RelativeLocations.Behind, Into);
                        EnumerateContainer(o, RelativeLocations.Under, Into);

                        if (o is Actor) 
                            EnumerateContainer(o, RelativeLocations.Player, Into);
                    }

                    return EnumerateObjectsControl.Continue;
                };
        }

        private static void EnumerateContainer(MudObject Container, RelativeLocations Location, List<MudObject> Into)
        {
            (Container as Container).EnumerateObjects(Location, (o, l) =>
                {
                        Into.Add(o);
                    return EnumerateObjectsControl.Continue;
                });
        }
    }
}
