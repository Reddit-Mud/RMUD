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
        public static void SendMessage(Actor Actor, String Message)
        {
            DatabaseLock.WaitOne();
            if (Actor != null && Actor.ConnectedClient != null)
                PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, Message));
            DatabaseLock.ReleaseMutex();
        }

        public static void SendLocaleMessage(MudObject Object, String Message)
        {
            DatabaseLock.WaitOne();
            var room = Mud.FindLocale(Object) as Room;
            if (room != null)
            {
                room.EnumerateObjects(RelativeLocations.Contents, (MudObject, loc) =>
                {
                    if (MudObject is Actor && (MudObject as Actor).ConnectedClient != null)
                        PendingMessages.Add(new RawPendingMessage((MudObject as Actor).ConnectedClient, Message));
                    return EnumerateObjectsControl.Continue;
                });
            }
            DatabaseLock.ReleaseMutex();
        }

        public static void SendExternalMessage(Actor Actor, String Message)
        {
            if (Actor == null) return;
            var location = Actor.Location as Room;
            if (location == null) return;

            DatabaseLock.WaitOne();
            location.EnumerateObjects(RelativeLocations.Contents, (o, l) =>
                {
                    var other = o as Actor;
                    if (other == null) return EnumerateObjectsControl.Continue;
                    if (Object.ReferenceEquals(other, Actor)) return EnumerateObjectsControl.Continue;
                    if (other.ConnectedClient == null) return EnumerateObjectsControl.Continue;
                    PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Message));
                    return EnumerateObjectsControl.Continue;
                });
            DatabaseLock.ReleaseMutex();
        }

        public static void SendMessage(Client Client, String Message)
        {
            DatabaseLock.WaitOne();
            PendingMessages.Add(new RawPendingMessage(Client, Message));
            DatabaseLock.ReleaseMutex();
        }

        public static void SendGlobalMessage(String Message)
        {
            DatabaseLock.WaitOne();
            foreach (var client in ConnectedClients)
            {
                if (client.IsLoggedOn)
                    PendingMessages.Add(new RawPendingMessage(client, Message));
            }
            DatabaseLock.ReleaseMutex();
        }
    }
}
