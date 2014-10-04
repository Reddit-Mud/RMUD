using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        public static bool ObjectContainsObject(MudObject Super, MudObject Sub)
        {
            if (Object.ReferenceEquals(Super, Sub)) return false; //Objects can't contain themselves...
            if (Sub.Location == null) return false;
            if (Object.ReferenceEquals(Super, Sub.Location)) return true;
            return ObjectContainsObject(Super, Sub.Location);
        }
    }
}
