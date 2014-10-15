using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class TelnetClient : Client
    {
        public System.Net.Sockets.Socket Socket = null;
        public String CommandQueue = "";
        public byte[] Storage = new byte[1024];
        internal bool WasRejected = false;

		public override string ConnectionDescription
		{
			get
			{
				if (Socket != null) return Socket.RemoteEndPoint.ToString();
				return "NOT CONNECTED";
			}
		}

        public override string IPString
        {
            get
            {
                //Why the fuck can't I get the IP address of a remote end point?
                var desc = ConnectionDescription;
                var split = desc.IndexOf(':');
                if (split == -1) return "0.0.0.0";
                return desc.Substring(0, split);
            }
        }

        private static byte[] SendBuffer = new byte[1024];

        override public void Send(String message)
        {
			if (Socket == null) return;

			try
			{
				int bytesSent = 0;

				if (!String.IsNullOrEmpty(CommandQueue)) message = "\r" + message + CommandQueue;

				while (bytesSent < message.Length)
				{
					int thisChunk = 0;
					for (int i = bytesSent; i < message.Length && thisChunk < 1024; ++i, ++thisChunk)
						SendBuffer[thisChunk] = (byte)message[i];
					if (Socket != null && Socket.Connected) Socket.Send(SendBuffer, thisChunk, System.Net.Sockets.SocketFlags.None);
					bytesSent += thisChunk;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Lost telnet client: " + this.Socket.RemoteEndPoint.ToString());
				Console.WriteLine(e.Message);

				this.Socket = null;
                if (!WasRejected) Mud.ClientDisconnected(this);
			}
        }

        override public void Disconnect()
        {
			if (Socket != null)
			{
				Console.WriteLine("Telnet client left gracefully : " + Socket.RemoteEndPoint.ToString());
				Socket.Close();
                Socket = null;
                //if (!WasRejected) Mud.ClientDisconnected(this);
			}
        }
    }
}
