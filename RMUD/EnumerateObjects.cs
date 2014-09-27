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

    [Flags]
    public enum EnumerateObjectsSettings
    {
        None = 0,
        SingleRecurse = 1,
        DeepRecurse = 2,
        IncludeSelf = 4,
        VisibleOnly = 8,
        Peers = 16,
        Location = 32,
        OnlySelf = 64,

        DeepLocation = DeepRecurse | Location | IncludeSelf,
        ShallowLocation = SingleRecurse | Location | IncludeSelf,
        Room = IncludeSelf | DeepRecurse,
    }

    public static partial class Mud
    {
        private static EnumerateObjectsControl __EnumerateObjects(MudObject Source, EnumerateObjectsSettings Settings, Func<MudObject, EnumerateObjectsControl> Callback)
        {
            if ((Settings & EnumerateObjectsSettings.OnlySelf) == EnumerateObjectsSettings.OnlySelf)
                return Callback(Source);

            if ((Settings & EnumerateObjectsSettings.Location) == EnumerateObjectsSettings.Location)
            {
                if (Source is Thing && (Source as Thing).Location != null)
                    return __EnumerateObjects((Source as Thing).Location, Settings ^ EnumerateObjectsSettings.Location, Callback);
                return EnumerateObjectsControl.Continue;
            }

            if ((Settings & EnumerateObjectsSettings.Peers) == EnumerateObjectsSettings.Peers)
            {
                if (Source is Thing && (Source as Thing).Location != null)
                    return __EnumerateObjects((Source as Thing).Location, Settings ^ EnumerateObjectsSettings.Peers, Callback);
                return EnumerateObjectsControl.Continue;
            }
             
            if ((Settings & EnumerateObjectsSettings.IncludeSelf) == EnumerateObjectsSettings.IncludeSelf)
                    if (Callback(Source) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;

            
            var container = Source as IContainer;
            if (container == null) return EnumerateObjectsControl.Continue;

            foreach (var thing in container)
            {
                if (Callback(thing) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;

                if ((Settings & EnumerateObjectsSettings.SingleRecurse) == EnumerateObjectsSettings.SingleRecurse && thing is IContainer)
                {
                    if (__EnumerateObjects(thing, Settings ^ EnumerateObjectsSettings.SingleRecurse, Callback) == EnumerateObjectsControl.Stop)
                        return EnumerateObjectsControl.Stop;
                }
                else if ((Settings & EnumerateObjectsSettings.DeepRecurse) == EnumerateObjectsSettings.DeepRecurse && thing is IContainer)
                {
                    if (__EnumerateObjects(thing, Settings, Callback) == EnumerateObjectsControl.Stop)
                        return EnumerateObjectsControl.Stop;
                }
            }

            return EnumerateObjectsControl.Continue;
        }

        public static void EnumerateObjects(MudObject Source, EnumerateObjectsSettings Settings, Func<MudObject, EnumerateObjectsControl> Callback)
        {
            __EnumerateObjects(Source, Settings, Callback);
        }
    }

}