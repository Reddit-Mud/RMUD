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
        internal static List<MudObject> ObjectsRegisteredForHeartbeat = new List<MudObject>();
        internal static UInt64 HeartbeatID = 0;
        internal static DateTime TimeOfLastHeartbeat = DateTime.Now;
        internal static DateTime TimeOfDay = DateTime.Parse("03/15/2015 11:15:00 -5:00");
		
        public static void RegisterForHeartbeat(MudObject Object)
        {
            if (!ObjectsRegisteredForHeartbeat.Contains(Object))
                ObjectsRegisteredForHeartbeat.Add(Object);
        }

        public static void UnregisterForHeartbeat(MudObject Object)
        {
            ObjectsRegisteredForHeartbeat.Remove(Object);
        }

        internal static void Heartbeat()
        {
            var now = DateTime.Now;
            var timeSinceLastBeat = now - TimeOfLastHeartbeat;
            if (timeSinceLastBeat.TotalMilliseconds >= Mud.SettingsObject.HeartbeatInterval)
            {
                TimeOfDay += Mud.SettingsObject.ClockAdvanceRate;

                TimeOfLastHeartbeat = now;
                HeartbeatID += 1;

                for (int i = 0; i < ObjectsRegisteredForHeartbeat.Count; )
                {
                    var Object = ObjectsRegisteredForHeartbeat[i];
                    if (Object.State == ObjectState.Destroyed)
                    {
                        ObjectsRegisteredForHeartbeat.RemoveAt(i);
                        continue;
                    }

                    Object.Heartbeat(HeartbeatID);
                    ++i;
                }

                Mud.SendPendingMessages();
            }
        }
    }
}
