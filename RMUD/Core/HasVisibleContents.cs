using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        public static bool HasVisibleContents(MudObject Object)
        {
            var container = Object as Container;
            if (container == null) return false;

            if ((container.LocationsSupported & RelativeLocations.In) == RelativeLocations.In)
            {
                return IsOpen(Object);
            }

            return false;
        }

        public static bool IsOpen(MudObject Object)
        {
            if (GlobalRules.ConsiderValueRule<bool>("openable", Object, Object))
                return GlobalRules.ConsiderValueRule<bool>("is-open", Object, Object);
            return true;
        }

        public static bool IsVisibleTo(MudObject Actor, MudObject Object)
        {
            var ceilingActor = Mud.FindLocale(Actor);
            if (ceilingActor == null) return false;

            var ceilingObject = Mud.FindLocale(Object);
            
            if (!System.Object.ReferenceEquals(ceilingObject, Object)) 
                return System.Object.ReferenceEquals(ceilingActor, ceilingObject);
            else if (ceilingActor is Room)
                    return (ceilingActor as Room).Links.Count(l => System.Object.ReferenceEquals(l.Portal, Object)) > 0;
            
            return false;
        }
    }
}
