﻿using System;
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

        /// <summary>
        /// Begin watching for output.
        /// </summary>
        public static void BeginOutputQuery()
        {
            OutputQueryTriggered = false;
        }

        /// <summary>
        /// Has there been any output since the last time BeginOutputQuery was called?
        /// </summary>
        /// <returns></returns>
        public static bool CheckOutputQuery()
        {
            return OutputQueryTriggered;
        }
    }

    public partial class MudObject
    {
        public static void SendMessage(Actor Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor != null && Actor.ConnectedClient != null)
                Core.PendingMessages.Add(new PendingMessage(Actor.ConnectedClient, Core.FormatMessage(Actor, Message, MentionedObjects)));
        }

        public static void SendMessage(MudObject MudObject, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (MudObject is Actor)
                SendMessage(MudObject as Actor, Message, MentionedObjects);
        }

        public static void SendLocaleMessage(MudObject Object, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            var container = MudObject.FindLocale(Object) as Container;
            if (container != null)
                foreach (var actor in container.EnumerateObjects<Actor>().Where(a => a.ConnectedClient != null))
                    Core.PendingMessages.Add(new PendingMessage(actor.ConnectedClient, Core.FormatMessage(actor, Message, MentionedObjects)));
        }

        public static void SendExternalMessage(Actor Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor == null) return;
            var location = Actor.Location as Room;
            if (location == null) return;

            foreach (var other in location.EnumerateObjects<Actor>().Where(a => !Object.ReferenceEquals(a, Actor) && (a.ConnectedClient != null)))
                Core.PendingMessages.Add(new PendingMessage(other.ConnectedClient, Core.FormatMessage(other, Message, MentionedObjects)));
                
        }

        public static void SendExternalMessage(MudObject Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            SendExternalMessage(Actor as Actor, Message, MentionedObjects);
        }


        public static void SendMessage(Client Client, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            Core.PendingMessages.Add(new PendingMessage(Client, Core.FormatMessage(Client.Player, Message, MentionedObjects)));
        }
    }
}
