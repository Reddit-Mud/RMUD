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
       internal static List<MudObject> MarkedObjects = new List<MudObject>();
		
        public static void MarkLocaleForUpdate(MudObject Object)
        {
            MudObject locale = FindLocale(Object);
            if (locale != null && !MarkedObjects.Contains(locale))
                MarkedObjects.Add(locale);
        }

        internal static void UpdateMarkedObjects()
        {
            var startCount = MarkedObjects.Count;
            for (int i = 0; i < startCount; ++i)
                MarkedObjects[i].HandleMarkedUpdate();
            MarkedObjects.RemoveRange(0, startCount);
        }
    }
}
