using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class MatchContext
    {
        public Actor ExecutingActor;

        private List<MatchableObject> CachedObjectsInScope = null;
        public List<MatchableObject> ObjectsInScope
        {
            get
            {
                if (CachedObjectsInScope != null) return CachedObjectsInScope;
                CachedObjectsInScope = new List<MatchableObject>();

                var location = ExecutingActor.Location as Room;
                if (location != null)
                    location.EnumerateObjects(RelativeLocations.Room, EnumerateCallback(CachedObjectsInScope));

                return CachedObjectsInScope;
            }
        }

        private static Func<MudObject, RelativeLocations, EnumerateObjectsControl> EnumerateCallback(List<MatchableObject> Into)
        {
            return (o, l) =>
                {
                        Into.Add(new MatchableObject(o, l));

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

        private static void EnumerateContainer(Container Container, RelativeLocations Location, List<MatchableObject> Into)
        {
            Container.EnumerateObjects(Location, (o, l) =>
                {
                        Into.Add(new MatchableObject(o, l));
                    return EnumerateObjectsControl.Continue;
                });
        }
    }
}
