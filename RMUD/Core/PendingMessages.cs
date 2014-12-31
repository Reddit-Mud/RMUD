using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    internal struct RawPendingMessage
    {
        public Client Destination;
        public String Message;

        public RawPendingMessage(Client Destination, String Message)
        {
            this.Destination = Destination;
            this.Message = Message;
        }
    }

    public static partial class Core
    {
		internal static List<RawPendingMessage> PendingMessages = new List<RawPendingMessage>();
                
        internal static void SendPendingMessages()
        {
			foreach (var message in PendingMessages)
				message.Destination.Send(message.Message + "\r\n");
            PendingMessages.Clear();
        }

		internal static void ClearPendingMessages()
		{
			PendingMessages.Clear();
		}

    }
}
