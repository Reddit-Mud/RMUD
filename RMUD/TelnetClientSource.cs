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

		public override string ConnectionDescription
		{
			get
			{
				if (Socket != null) return Socket.RemoteEndPoint.ToString();
				return "NOT CONNECTED";
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
				Mud.ClientDisconnected(this);
			}
        }

        override public void Disconnect()
        {
			if (Socket != null)
			{
				Console.WriteLine("Telnet client left gracefully : " + Socket.RemoteEndPoint.ToString());
				Socket.Close();
			}
			Socket = null;
			Mud.ClientDisconnected(this);
        }
    }

    public class TelnetClientSource
    {
        public int Port = 8669;

        System.Net.Sockets.Socket ListenSocket = null;

        static System.Threading.Mutex ClientLock = new System.Threading.Mutex();
        static LinkedList<TelnetClient> Clients = new LinkedList<TelnetClient>();

        public void Listen()
        {
            ListenSocket = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Stream,
                System.Net.Sockets.ProtocolType.IP);

            ListenSocket.Bind(new System.Net.IPEndPoint(0, Port));
            ListenSocket.Listen(16);
            ListenSocket.BeginAccept(OnNewClient, null);

            Console.WriteLine("Listening on port " + Port);
        }

		public void Shutdown()
		{
			ListenSocket.Close();
		}

        void OnNewClient(IAsyncResult _asyncResult)
        {
            System.Net.Sockets.Socket ClientSocket = ListenSocket.EndAccept(_asyncResult);
            ListenSocket.BeginAccept(OnNewClient, null);

            var NewClient = new TelnetClient { Socket = ClientSocket };
            ClientSocket.BeginReceive(NewClient.Storage, 0, 1024, System.Net.Sockets.SocketFlags.Partial, OnData, NewClient);
            Console.WriteLine("New telnet client: " + ClientSocket.RemoteEndPoint.ToString());
            Mud.ClientConnected(NewClient);
        }

        private static string ValidCharacters = "@=|^\\;?:#.,!\"'$*<>/()[]{}-_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        void OnData(IAsyncResult _asyncResult)
        {
            var Client = _asyncResult.AsyncState as TelnetClient;
            System.Net.EndPoint remoteEndPoint = null;

            try
            {
                remoteEndPoint = Client.Socket.RemoteEndPoint;

                System.Net.Sockets.SocketError Error;
                int DataSize = Client.Socket.EndReceive(_asyncResult, out Error);

                if (DataSize == 0 || Error != System.Net.Sockets.SocketError.Success)
                {
                    if (remoteEndPoint != null)
                        Console.WriteLine("Lost telnet client: " + remoteEndPoint.ToString());
                    else
                        Console.WriteLine("Lost telnet client: Unknown remote endpoint.");

                    Mud.ClientDisconnected(Client);
                }
                else
                {
                    for (int i = 0; i < DataSize; ++i)
                    {
                        if (Client.Storage[i] == '\n' || Client.Storage[i] == '\r')
                        {
                            if (!String.IsNullOrEmpty(Client.CommandQueue))
                            {
                                String Command = Client.CommandQueue;
                                Client.CommandQueue = "";
                                Mud.EnqueuClientCommand(Client, Command);
                            }
                        }
                        else if (Client.Storage[i] == '\b')
                        {
                            if (Client.CommandQueue.Length > 0)
                                Client.CommandQueue = Client.CommandQueue.Remove(Client.CommandQueue.Length - 1);
                        }
                        else if (ValidCharacters.Contains((char)Client.Storage[i]))
                            Client.CommandQueue += (char)Client.Storage[i];
                    }

                    Client.Socket.BeginReceive(Client.Storage, 0, 1024, System.Net.Sockets.SocketFlags.Partial, OnData, Client);
                }
            }
            catch (Exception e)
            {
                if (remoteEndPoint != null)
                    Console.WriteLine("Lost telnet client: " + remoteEndPoint.ToString());
                else
                    Console.WriteLine("Lost telnet client: Unknown remote endpoint.");
				Console.WriteLine(e.Message);
                Mud.ClientDisconnected(Client);
            }
        }
    }
}
