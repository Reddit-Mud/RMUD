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
        public static bool SilentFlag = false;

        public static String FormatMessage(Actor Recipient, String Message, params MudObject[] Objects)
        {
            for (int i = 0; i < Objects.Length; ++i)
            {
                Message = Message.Replace("<the" + i + ">", Objects[i].Definite(Recipient));
                Message = Message.Replace("<a" + i + ">", Objects[i].Indefinite(Recipient));                
            }

            var builder = new StringBuilder();
            var cap = false;
            for (int i = 0; i < Message.Length; ++i)
            {
                if (Message[i] == '^') cap = true;
                else
                {
                    if (cap) builder.Append(new String(Message[i], 1).ToUpper());
                    else builder.Append(Message[i]);
                    cap = false;
                }
            }

            return builder.ToString();
        }

        public static void SendMessage(Actor Actor, String Message, params MudObject[] MentionedObjects)
        {
            if (Actor != null && Actor.ConnectedClient != null)
                PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, FormatMessage(Actor, Message, MentionedObjects)));
        }

        public static void SendMessage(MudObject MudObject, String Message, params MudObject[] MentionedObjects)
        {
            if (MudObject is Actor)
                SendMessage(MudObject as Actor, Message, MentionedObjects);
        }

        public static void SendLocaleMessage(MudObject Object, String Message, params MudObject[] MentionedObjects)
        {
            var container = Mud.FindLocale(Object) as Container;
            if (container != null)
            {
                container.EnumerateObjects(RelativeLocations.EveryMudObject, (MudObject, loc) =>
                {
                    var actor = MudObject as Actor;
                    if (actor != null && actor.ConnectedClient != null)
                        PendingMessages.Add(new RawPendingMessage(actor.ConnectedClient, FormatMessage(actor, Message, MentionedObjects)));
                    return EnumerateObjectsControl.Continue;
                });
            }
        }

        public static void SendExternalMessage(Actor Actor, String Message, params MudObject[] MentionedObjects)
        {
            if (Actor == null) return;
            var location = Actor.Location as Room;
            if (location == null) return;

            location.EnumerateObjects(RelativeLocations.Contents, (o, l) =>
                {
                    var other = o as Actor;
                    if (other == null) return EnumerateObjectsControl.Continue;
                    if (Object.ReferenceEquals(other, Actor)) return EnumerateObjectsControl.Continue;
                    if (other.ConnectedClient == null) return EnumerateObjectsControl.Continue;
                    PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, FormatMessage(other, Message, MentionedObjects)));
                    return EnumerateObjectsControl.Continue;
                });
        }

        public static void SendExternalMessage(MudObject Actor, String Message, params MudObject[] MentionedObjects)
        {
            SendExternalMessage(Actor as Actor, Message, MentionedObjects);
        }


        public static void SendMessage(Client Client, String Message)
        {
            PendingMessages.Add(new RawPendingMessage(Client, Message));
        }

        public static void SendGlobalMessage(String Message)
        {
            foreach (var client in ConnectedClients)
            {
                if (client.IsLoggedOn)
                    PendingMessages.Add(new RawPendingMessage(client, Message));
            }
        }
    }
}
