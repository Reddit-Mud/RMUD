using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
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
            if (Mud.ClientConnected(NewClient) == Mud.ClientAcceptanceStatus.Rejected)
            {
                NewClient.WasRejected = true;
                ClientSocket.Close();
            }
            else
            {
                ClientSocket.BeginReceive(NewClient.Storage, 0, 1024, System.Net.Sockets.SocketFlags.Partial, OnData, NewClient);
                Console.WriteLine("New telnet client: " + ClientSocket.RemoteEndPoint.ToString());
            }
        }

        private static string ValidCharacters = "@=|^\\;?:#.,!\"'$*<>/()[]{}-+_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        void OnData(IAsyncResult _asyncResult)
        {
            var Client = _asyncResult.AsyncState as TelnetClient;
            System.Net.EndPoint remoteEndPoint = null;

            if (Client.Socket == null) return;

            try
            {
                remoteEndPoint = Client.Socket.RemoteEndPoint;
            }
            catch (Exception) //Just shut this one up.
            {
                if (!Client.WasRejected) Mud.ClientDisconnected(Client);
                return;
            }

            try
            {
                System.Net.Sockets.SocketError Error;
                int DataSize = Client.Socket.EndReceive(_asyncResult, out Error);

                if (DataSize == 0 || Error != System.Net.Sockets.SocketError.Success)
                {
                    if (remoteEndPoint != null)
                        Console.WriteLine("Lost telnet client: " + remoteEndPoint.ToString());
                    else
                        Console.WriteLine("Lost telnet client: Unknown remote endpoint.");

                    if (!Client.WasRejected) Mud.ClientDisconnected(Client);
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
                Console.WriteLine(e.StackTrace);
                if (!Client.WasRejected) Mud.ClientDisconnected(Client);
            }
        }
    }
}
