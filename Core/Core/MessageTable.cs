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

        private struct MessageDefinition
        {
            public MessageDefinitionPriority Priority;
            public String Message;
        }

        private static Dictionary<String, MessageDefinition> MessageDefinitions = new Dictionary<string, MessageDefinition>();

        public static void StandardMessage(String Name, String Contents)
        {
            if (MessageDefinitions.ContainsKey(Name) && MessageDefinitions[Name].Priority == MessageDefinitionPriority.Override) return;
            MessageDefinitions.Upsert(Name, new MessageDefinition { Message = Contents, Priority = MessageDefinitionPriority.Standard });
        }

        public static void OverrideMessage(String Name, String Contents)
        {
            MessageDefinitions.Upsert(Name, new MessageDefinition { Message = Contents, Priority = MessageDefinitionPriority.Override });
        }

        public static String Message(String Name)
        {
            if (MessageDefinitions.ContainsKey(Name)) return MessageDefinitions[Name].Message;
            else return Name;
        }

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
