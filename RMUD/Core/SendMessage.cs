using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Core
    {
        internal static bool SilentFlag = false;

        internal static bool OutputQueryTriggered = false;

        internal static void BeginOutputQuery()
        {
            OutputQueryTriggered = false;
        }

        internal static bool CheckOutputQuery()
        {
            return OutputQueryTriggered;
        }

        internal static String UnformattedItemList(int StartIndex, int Count)
        {
            var builder = new StringBuilder();
            for (int i = StartIndex; i < StartIndex + Count; ++i)
            {
                builder.Append("<a" + i + ">");
                if (i != (StartIndex + Count - 1)) builder.Append(", ");
            }
            return builder.ToString();
        }

        internal static String FormatMessage(Actor Recipient, String Message, params MudObject[] Objects)
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
    }

    public partial class MudObject
    {
        public static void SendMessage(Actor Actor, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor != null && Actor.ConnectedClient != null)
                Core.PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, Core.FormatMessage(Actor, Message, MentionedObjects)));
        }

        public static void SendMessage(MudObject MudObject, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (MudObject is Actor)
                SendMessage(MudObject as Actor, Message, MentionedObjects);
        }

        public static void SendLocaleMessage(MudObject Object, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            var container = MudObject.FindLocale(Object) as Container;
            if (container != null)
                foreach (var actor in container.EnumerateObjects<Actor>().Where(a => a.ConnectedClient != null))
                    Core.PendingMessages.Add(new RawPendingMessage(actor.ConnectedClient, Core.FormatMessage(actor, Message, MentionedObjects)));
        }

        public static void SendExternalMessage(Actor Actor, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor == null) return;
            var location = Actor.Location as Room;
            if (location == null) return;

            foreach (var other in location.EnumerateObjects<Actor>().Where(a => !Object.ReferenceEquals(a, Actor) && (a.ConnectedClient != null)))
                Core.PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Core.FormatMessage(other, Message, MentionedObjects)));
                
        }

        public static void SendExternalMessage(MudObject Actor, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            SendExternalMessage(Actor as Actor, Message, MentionedObjects);
        }


        public static void SendMessage(Client Client, String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            Core.PendingMessages.Add(new RawPendingMessage(Client,
                Client.IsLoggedOn ?
                    Core.FormatMessage(Client.Account.LoggedInCharacter, Message, MentionedObjects) :
                    Message));
        }

        public static void SendGlobalMessage(String Message, params MudObject[] MentionedObjects)
        {
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            foreach (var client in Core.ConnectedClients)
            {
                if (client.IsLoggedOn)
                    SendMessage(client, Message, MentionedObjects);
            }
        }
    }
}
