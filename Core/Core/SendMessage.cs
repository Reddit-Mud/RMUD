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
        public static void SendMessage(MudObject Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor != null)
            {
                var client = Actor.GetPropertyOrDefault<Client>("client");
                if (client != null)
                    Core.PendingMessages.Add(new PendingMessage(client, Core.FormatMessage(Actor, Message, MentionedObjects)));
            }
        }

        public static void SendLocaleMessage(MudObject Object, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            var locale = MudObject.FindLocale(Object);
            foreach (var actor in locale.EnumerateObjects())
            {
                var client = actor.GetPropertyOrDefault<Client>("client");
                if (client != null)
                    Core.PendingMessages.Add(new PendingMessage(client, Core.FormatMessage(actor, Message, MentionedObjects)));
            }
        }

        public static void SendExternalMessage(MudObject Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor == null) return;
            if (Actor.Location == null) return;

            foreach (var other in Actor.Location.EnumerateObjects().Where(a => !Object.ReferenceEquals(a, Actor)))
            {
                var client = other.GetPropertyOrDefault<Client>("client");
                if (client != null)
                    Core.PendingMessages.Add(new PendingMessage(client, Core.FormatMessage(other, Message, MentionedObjects)));
            }
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
