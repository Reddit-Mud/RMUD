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
                CachedObjectsInScope = new List<MudObject>(MudObject.EnumerateVisibleTree(MudObject.FindLocale(ExecutingActor)).Where(thing => !Object.ReferenceEquals(thing, ExecutingActor)));
                return CachedObjectsInScope;
            }
        }
    }
}
