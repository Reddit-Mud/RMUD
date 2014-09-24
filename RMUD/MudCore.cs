using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        private static Mutex CommandLock = new Mutex();
        private static LinkedList<Action> PendingActions = new LinkedList<Action>();
        private static Thread ActionExecutionThread;
        internal static List<Client> ConnectedClients = new List<Client>();
        internal static Mutex DatabaseLock = new Mutex();
		private static bool ShuttingDown = false;

        private static String CriticalLog = "errors.log";

		internal static ParserCommandHandler ParserCommandHandler;
		public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
		internal static LoginCommandHandler LoginCommandHandler;

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

		internal static List<RawPendingMessage> PendingMessages = new List<RawPendingMessage>();
		
        internal static void EnqueuAction(Action Action)
        {
            CommandLock.WaitOne();
            PendingActions.AddLast(Action);
            CommandLock.ReleaseMutex();
        }

        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
			EnqueuAction(() => { Client.CommandHandler.HandleCommand(Client, RawCommand); });
        }

		public static void ClientDisconnected(Client client)
		{
			DatabaseLock.WaitOne();
			ConnectedClients.Remove(client);
			if (client.Player != null) client.Player.ConnectedClient = null;
			DatabaseLock.ReleaseMutex();
			EnqueuAction(() => {
				if (client.Player != null)
				{
					Thing.Move(client.Player, null);
				}			
			});
		}

        public static void ClientConnected(Client client)
        {
			client.Player = new Actor();
			client.Player.ConnectedClient = client;
			client.CommandHandler = LoginCommandHandler;

			var settings = GetObject("settings", s => client.Send(s + "\r\n")) as Settings;
			client.Send(settings.Banner);
			client.Send(settings.MessageOfTheDay);

			DatabaseLock.WaitOne();
            ConnectedClients.Add(client);
			DatabaseLock.ReleaseMutex();
        }

        public static bool Start(String basePath)
        {
            try
            {
				InitializeDatabase(basePath);
				var settings = GetObject("settings") as Settings;
				if (settings == null) throw new InvalidProgramException("No settings object is defined in the database!");

				ParserCommandHandler = new ParserCommandHandler();
				LoginCommandHandler = new LoginCommandHandler();
				
				ActionExecutionThread = new Thread(CommandProcessingThread);
                ActionExecutionThread.Start();

                Console.WriteLine("Engine ready with path " + basePath + ".");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start mud engine.");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw e;
            }
            return true;
        }

        public static void Shutdown()
        {
			ShuttingDown = true;
        }

		public static void SendEventMessage(Actor Actor, EventMessageScope Scope, String Message)
		{
            DatabaseLock.WaitOne();

			switch (Scope)
			{
				//Send message only to the player
				case EventMessageScope.Single:
					{
						if (Actor == null) break;
						if (Actor.ConnectedClient == null) break;
						PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, Message));
					}
					break;

				//Send message to everyone in the same location as the player
				case EventMessageScope.Local:
					{
						if (Actor == null) break;
						var location = Actor.Location as Room;
						if (location == null) break;
						foreach (var thing in location.Contents)
						{
							var other = thing as Actor;
							if (other == null) continue;
							if (other.ConnectedClient == null) continue;
							PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Message));
						}
					}
					break;

				//Send message to everyone in the same location EXCEPT the player.
				case EventMessageScope.External:
					{
						if (Actor == null) break;
						var location = Actor.Location as Room;
						if (location == null) break;
						foreach (var thing in location.Contents)
						{
							var other = thing as Actor;
							if (other == null) continue;
							if (Object.ReferenceEquals(other, Actor)) continue;
							if (other.ConnectedClient == null) continue;
							PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Message));
						}
					}
					break;
			}

            DatabaseLock.ReleaseMutex();
        }

        public static void CommandProcessingThread()
        {
            while (!ShuttingDown)
            {
                System.Threading.Thread.Sleep(10);

				while (PendingActions.Count > 0)
				{
					CommandLock.WaitOne();
					var PendingCommand = PendingActions.First.Value;
					PendingActions.RemoveFirst();
					CommandLock.ReleaseMutex();

                    DatabaseLock.WaitOne();
                    try
                    {
                        PendingCommand();
                        SendPendingMessages();
                    }
                    catch (Exception e)
                    {
                        LogCriticalError(e);
                        ClearPendingMessages();
                    }
                    DatabaseLock.ReleaseMutex();
                }
            }
        }

        public static void LogCriticalError(Exception e)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- Error while handling client command.", DateTime.Now);
            logfile.WriteLine(e.Message);
            logfile.WriteLine(e.StackTrace);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- Error while handling client command.", DateTime.Now);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        public static void LogError(String ErrorString)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- {1}", DateTime.Now, ErrorString);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- {1}", DateTime.Now, ErrorString);
        }

        internal static void SendPendingMessages()
        {
			foreach (var eventMessage in PendingMessages)
				eventMessage.Destination.Send(eventMessage.Message);
            PendingMessages.Clear();
        }

		internal static void ClearPendingMessages()
		{
			PendingMessages.Clear();
		}

    }
}
