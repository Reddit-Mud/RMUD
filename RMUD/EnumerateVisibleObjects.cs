using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum EnumerateVisibleObjectsControl
    {
        Stop,
        Continue
    }

    public enum EnumerateVisibleObjectsSettings
    {
        None,
        Recurse,
    }

    public static partial class Mud
    {
        private static EnumerateVisibleObjectsControl __EnumerateVisibleObjects(MudObject Source, EnumerateVisibleObjectsSettings Settings, Func<MudObject, EnumerateVisibleObjectsControl> Callback)
        {
            var container = Source as IContainer;
            if (container == null) return EnumerateVisibleObjectsControl.Continue;

            foreach (var thing in container)
            {
                if (Callback(thing) == EnumerateVisibleObjectsControl.Stop) return EnumerateVisibleObjectsControl.Stop;
                if (Settings == EnumerateVisibleObjectsSettings.Recurse && thing is IContainer)
                    if (__EnumerateVisibleObjects(thing, Settings, Callback) == EnumerateVisibleObjectsControl.Stop)
                        return EnumerateVisibleObjectsControl.Stop;
            }

            return EnumerateVisibleObjectsControl.Continue;
        }

        public static void EnumerateVisibleObjects(MudObject Source, EnumerateVisibleObjectsSettings Settings, Func<MudObject, EnumerateVisibleObjectsControl> Callback)
        {
            __EnumerateVisibleObjects(Source, Settings, Callback);
        }
    }

}