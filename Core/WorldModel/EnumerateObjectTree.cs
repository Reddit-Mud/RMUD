using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class MudObject
    {
        private static IEnumerable<MudObject> _enumerateObjectTree(MudObject C)
        {
            if (C != null)
            {
                yield return C;

                if (C is Container)
                    foreach (var item in (C as Container).EnumerateObjects())
                        foreach (var sub in _enumerateObjectTree(item))
                            yield return sub;
            }
        }

        public static IEnumerable<MudObject> EnumerateObjectTree(MudObject Root)
        {
            foreach (var item in _enumerateObjectTree(Root))
                yield return item;
        }

        private static IEnumerable<MudObject> _enumerateVisibleTree(MudObject C)
        {
            if (C != null)
            {
                yield return C;

                if (C is Container)
                    foreach (var list in (C as Container).Lists)
                    {
                        if (list.Key == RelativeLocations.In && C.GetBooleanProperty("openable?") && !C.GetBooleanProperty("open?")) continue;
                        foreach (var item in list.Value)
                            foreach (var sub in _enumerateVisibleTree(item))
                                yield return sub;
                    }
            }
        }

        public static IEnumerable<MudObject> EnumerateVisibleTree(MudObject Root)
        {
            foreach (var item in _enumerateVisibleTree(Root))
                yield return item;
        }
    }
}