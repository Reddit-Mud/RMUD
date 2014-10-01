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
            var container = Object as IContainer;
            if (container == null) return false;

            if ((container.LocationsSupported & RelativeLocations.In) == RelativeLocations.In)
            {
                var openable = Object as OpenableRules;
                if (openable != null) return openable.Open;
                return true;
            }

            return false;
        }

        public static bool IsOpen(MudObject Object)
        {
            var openable = Object as OpenableRules;
            if (openable != null) return openable.Open;
            return true;
        }

        public static bool IsVisibleTo(MudObject A, MudObject B)
        {
            var ceilingA = Mud.FindVisibilityCeiling(A);
            if (ceilingA == null) return false;

            var ceilingB = Mud.FindVisibilityCeiling(B);
            if (ceilingB == null) return false;

            return System.Object.ReferenceEquals(ceilingA, ceilingB);
        }
    }
}
