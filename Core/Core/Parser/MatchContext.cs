using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    /// <summary>
    /// Represents the context the parser is attempting to make a match within.
    /// </summary>
    public class MatchContext
    {
        public Actor ExecutingActor;

        /// <summary>
        /// The objects in scope will not change while matching a command, as command matchers by definition
        /// must not alter the state of the world. We cache to prevent two problems that occur in Inform7:
        ///     A) Calculating the in-scope objects multiple times each command, and
        ///     B) Calculating them at all, when the command won't need them.
        /// Additionally, since the same context is used for every command matcher that is tried, the scope
        /// needs only be calculated once.
        /// </summary>
        private List<MudObject> CachedObjectsInScope = null;
        public List<MudObject> ObjectsInScope
        {
            get
            {
                if (CachedObjectsInScope != null) return CachedObjectsInScope;
                
                // Exclude the actor whose scope we are calculating from that scope.
                CachedObjectsInScope = new List<MudObject>(MudObject.EnumerateVisibleTree(MudObject.FindLocale(ExecutingActor)).Where(thing => !Object.ReferenceEquals(thing, ExecutingActor)));

                return CachedObjectsInScope;
            }
        }
    }
}
