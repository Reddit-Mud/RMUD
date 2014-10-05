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
		
        public static void RegisterForHeartbeat(MudObject Object)
        {
            DatabaseLock.WaitOne();
            if (!ObjectsRegisteredForHeartbeat.Contains(Object))
                ObjectsRegisteredForHeartbeat.Add(Object);
            DatabaseLock.ReleaseMutex();
        }

        public static void UnregisterForHeartbeat(MudObject Object)
        {
            DatabaseLock.WaitOne();
            ObjectsRegisteredForHeartbeat.Remove(Object);
            DatabaseLock.ReleaseMutex();
        }

        internal static void Heartbeat()
        {
            var now = DateTime.Now;
            var timeSinceLastBeat = now - TimeOfLastHeartbeat;
            if (timeSinceLastBeat.TotalMilliseconds >= Mud.SettingsObject.HeartbeatInterval)
            {
                TimeOfLastHeartbeat = now;
                HeartbeatID += 1;

                DatabaseLock.WaitOne();
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
                DatabaseLock.ReleaseMutex();
            }
        }
    }
}
