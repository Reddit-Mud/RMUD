using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public static partial class Core
    {
        private enum MessageDefinitionPriority
        {
            Standard,
            Override
        }

        /// <summary>
        /// Represents an entry in the message table.
        /// </summary>
        private struct MessageDefinition
        {
            public MessageDefinitionPriority Priority;
            public String Message;
        }

        private static Dictionary<String, MessageDefinition> MessageDefinitions = new Dictionary<string, MessageDefinition>();

        /// <summary>
        /// Create a standard message. If a standard message with this name already exists, replace it.
        /// If an override message with this name already exists, do not replace it.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Contents"></param>
        public static void StandardMessage(String Name, String Contents)
        {
            if (MessageDefinitions.ContainsKey(Name) && MessageDefinitions[Name].Priority == MessageDefinitionPriority.Override) return;
            MessageDefinitions.Upsert(Name, new MessageDefinition { Message = Contents, Priority = MessageDefinitionPriority.Standard });
        }

        /// <summary>
        /// Create or override a standard message. Any message already defined is replaced. Additionally, future calls
        /// to StandardMessage will not replace this message.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Contents"></param>
        public static void OverrideMessage(String Name, String Contents)
        {
            MessageDefinitions.Upsert(Name, new MessageDefinition { Message = Contents, Priority = MessageDefinitionPriority.Override });
        }

        /// <summary>
        /// Retrieve a standard message from it's name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>The message, or Name if there is no message by that name</returns>
        public static String GetMessage(String Name)
        {
            if (MessageDefinitions.ContainsKey(Name)) return MessageDefinitions[Name].Message;
            else return Name;
        }

        /// <summary>
        /// Emits code to customize all messages. This is a development aid and is not used in normal operation
        /// of a mud.
        /// </summary>
        /// <param name="Into"></param>
        public static void DumpMessagesForCustomization(StringBuilder Into)
        {
            foreach (var message in MessageDefinitions)
            {
                Into.Append("RMUD.Core.OverrideMessage(\"");
                Into.Append(message.Key);
                Into.Append("\", \"");
                Into.Append(message.Value.Message);
                Into.Append("\");");
                Into.AppendLine();
            }
        }
    }
}
