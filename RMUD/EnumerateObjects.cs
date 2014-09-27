using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum EnumerateObjectsControl
    {
        Stop,
        Continue
    }

    public enum EnumerateObjectsDepth
    {
        None,
        Shallow,
        Deep
    }

    public static partial class Mud
    {
        public static EnumerateObjectsControl EnumerateObjects(MudObject Source, EnumerateObjectsDepth Depth, Func<MudObject, EnumerateObjectsControl> Callback)
        {
            var container = Source as IContainer;
            if (container == null) return EnumerateObjectsControl.Continue;

            foreach (var thing in container)
            {
                if (Callback(thing) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;

                if (Depth == EnumerateObjectsDepth.Deep)
                {
                    if (EnumerateObjects(thing, EnumerateObjectsDepth.Deep, Callback) == EnumerateObjectsControl.Stop)
                        return EnumerateObjectsControl.Stop;
                }
                else if (Depth == EnumerateObjectsDepth.Shallow)
                {
                    if (EnumerateObjects(thing, EnumerateObjectsDepth.None, Callback) == EnumerateObjectsControl.Stop)
                        return EnumerateObjectsControl.Stop;
                }
            }

            return EnumerateObjectsControl.Continue;
        }
    }

}