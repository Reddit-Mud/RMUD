using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace RMUD.Modules.Network.Telnet
{
    public class TelnetClientSource
    {
        public int Port = 8669;

        System.Net.Sockets.Socket ListenSocket = null;

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

        private const int BytesPerLong = 4; // 32 / 8
        private const int BitsPerByte = 8;
     
        /// <summary>
        /// Sets the keep-alive interval for the socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="time">Time between two keep alive "pings".</param>
        /// <param name="interval">Time between two keep alive "pings" when first one fails.</param>
        /// <returns>If the keep alive infos were succefully modified.</returns>
        private static bool SetKeepAlive(Socket socket, ulong time, ulong interval)
        {
            try
            {
                // Array to hold input values.
                var input = new[] {
                    (time == 0 || interval == 0) ? 0UL : 1UL, // on or off
                    time,
                    interval
                };

                // Pack input into byte struct.
                byte[] inValue = new byte[3 * BytesPerLong];
                for (int i = 0; i < input.Length; i++)
                {
                    inValue[i * BytesPerLong + 3] = (byte)(input[i] >> ((BytesPerLong - 1) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 2] = (byte)(input[i] >> ((BytesPerLong - 2) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 1] = (byte)(input[i] >> ((BytesPerLong - 3) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 0] = (byte)(input[i] >> ((BytesPerLong - 4) * BitsPerByte) & 0xff);
                }

                // Create bytestruct for result (bytes pending on server socket).
                byte[] outValue = BitConverter.GetBytes(0);

                // Write SIO_VALS to Socket IOControl.
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                socket.IOControl(IOControlCode.KeepAliveValues, inValue, outValue);
            }
            catch (SocketException e)
            {
                Core.LogCriticalError(e);
                Core.LogError("Failed to set keep-alive: " + e.ErrorCode);
                return false;
            }

            return true;
        }

        void OnNewClient(IAsyncResult _asyncResult)
        {
            System.Net.Sockets.Socket ClientSocket = ListenSocket.EndAccept(_asyncResult);
            ListenSocket.BeginAccept(OnNewClient, null);

            SetKeepAlive(ClientSocket, 1000 * 60, 1000);

            var NewClient = new TelnetClient { Socket = ClientSocket };
            if (Modules.Network.Clients.ClientConnected(NewClient) == Modules.Network.ClientAcceptanceStatus.Rejected)
            {
                NewClient.WasRejected = true;
                ClientSocket.Close();
            }
            else
            {
                // We will handle all the echoing echoing echoing
                //var echoCommand = new byte[]
                //{ 
                //    (byte)TelnetControlCodes.IAC, 
                //    (byte)TelnetControlCodes.Will, // TODO: Handle client response denying this request
                //    (byte)TelnetControlCodes.Echo,
                //    (byte)TelnetControlCodes.IAC,
                //    (byte)TelnetControlCodes.Dont,
                //    (byte)TelnetControlCodes.Echo,
                //};
                //ClientSocket.Send(echoCommand);

                ClientSocket.BeginReceive(NewClient.Storage, 0, 1024, System.Net.Sockets.SocketFlags.Partial, OnData, NewClient);
                Console.WriteLine("New telnet client: " + ClientSocket.RemoteEndPoint.ToString());
            }
        }

        private static string ValidCharacters = "@=|^\\;?:#.,!\"'$*<>/()[]{}-+_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        void OnData(IAsyncResult _asyncResult)
        {
            var Client = _asyncResult.AsyncState as TelnetClient;
            System.Net.EndPoint remoteEndPoint = null;

            if (Client.Socket == null)
            {
                if (!Client.WasRejected) Modules.Network.Clients.ClientDisconnected(Client);
                return;
            }

            try
            {
                remoteEndPoint = Client.Socket.RemoteEndPoint;
            }
            catch (Exception) //Just shut this one up.
            {
                if (!Client.WasRejected) Modules.Network.Clients.ClientDisconnected(Client);
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

                    if (!Client.WasRejected) Modules.Network.Clients.ClientDisconnected(Client);
                }
                else
                {
                    for (int i = 0; i < DataSize; ++i)
                    {
                        var character = (char)Client.Storage[i];
                        
                        if (character == '\n' || character == '\r')
                        {
                            //if (Client.Echo == Echo.All || Client.Echo == Echo.Mask)
                            //{
                            //    Client.Send(new byte[] { (byte)character });
                            //}

                            if (!String.IsNullOrEmpty(Client.CommandQueue))
                            {
                                String Command = Client.CommandQueue;
                                Client.CommandQueue = "";
                                Core.EnqueuActorCommand(Client.Player, Command);
                            }
                        }
                        else if (character == '\b')
                        {
                            if (Client.CommandQueue.Length > 0)
                            {
                                Client.CommandQueue = Client.CommandQueue.Remove(Client.CommandQueue.Length - 1);
                                //if (Client.Echo == Echo.All || Client.Echo == Echo.Mask)
                                //{
                                //    Client.Send(new byte[] { (byte)character });
                                //}
                            }
                        }
                        else if (ValidCharacters.Contains(character))
                        {
                            Client.CommandQueue += character;
                            //switch (Client.Echo)
                            //{
                            //    case Echo.All: Client.Send(new byte[] { (byte)character }); break;
                            //    case Echo.Mask: Client.Send(new byte[] { (byte)'*' }); break;
                            //    case Echo.None: break;
                            //}
                        }

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
                if (!Client.WasRejected) Modules.Network.Clients.ClientDisconnected(Client);
            }
        }
    }
}
