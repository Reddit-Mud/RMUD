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
                {
                    Mud.EnumerateObjects(location, EnumerateObjectsDepth.Shallow, (o, l) =>
                        {
                            if (o is IMatchable)
                                CachedObjectsInScope.Add(new MatchableObject(o as IMatchable, l));
                            return EnumerateObjectsControl.Continue;
                        });
                }

                return CachedObjectsInScope;
            }
        }

    }
}
