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
        public static bool IsVisibleTo(MudObject Actor, MudObject Object)
        {
            var ceilingActor = Mud.FindLocale(Actor);
            if (ceilingActor == null) return false;

            if (Object is Portal)
            {
                var ceilingA = Mud.FindLocale((Object as Portal).FrontSide);
                var ceilingB = Mud.FindLocale((Object as Portal).BackSide);

                return System.Object.ReferenceEquals(ceilingActor, ceilingA) || System.Object.ReferenceEquals(ceilingActor, ceilingB);
            }
            else
            {
                var ceilingObject = Mud.FindLocale(Object);
                return System.Object.ReferenceEquals(ceilingActor, ceilingObject);
            }
        }
    }
}
