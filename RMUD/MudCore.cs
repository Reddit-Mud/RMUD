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
        private static List<Client> ConnectedClients = new List<Client>();
        private static Mutex DatabaseLock = new Mutex();

		internal static ParserCommandHandler ParserCommandHandler;
		public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
		internal static LoginCommandHandler LoginCommandHandler;

		internal struct RawPendingMessage
		{
			public Client Destination;
			public String Message;
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
			DatabaseLock.ReleaseMutex();
			EnqueuAction(() => { });
		}

        public static void ClientConnected(Client client)
        {
			client.Player = new Actor();
			client.Player.ConnectedClient = client;
			client.CommandHandler = LoginCommandHandler;

			var settings = GetObject("settings") as Settings;
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

        public static void Join()
        {
            ActionExecutionThread.Join();
        }

		public static void SendEventMessage(Actor Actor, EventMessageScope Scope, String Message, params Object[] Arguments)
		{
            DatabaseLock.WaitOne();

			var args = new List<Object>();
			args.Add(Actor);
			args.AddRange(Arguments);

			switch (Scope)
			{
				//Send message only to the player
				case EventMessageScope.Private:
					{
						if (Actor == null) break;
						if (Actor.ConnectedClient == null) break;
						args[0] = "you";
						PendingMessages.Add(new RawPendingMessage
						{
							Destination = Actor.ConnectedClient,
							Message = String.Format(Message, args.ToArray())
						});
					}
					break;

				//Send message to everyone in the same location as the player
				case EventMessageScope.Locality:
					{
						if (Actor == null) break;
						var location = Actor.Location as Room;
						if (location == null) break;
						foreach (var thing in location.Contents)
						{
							var other = thing as Actor;
							if (other == null) continue;
							if (other.ConnectedClient == null) continue;
							if (Object.ReferenceEquals(Actor, other))
								args[0] = "you";
							else
								args[0] = Actor;
							PendingMessages.Add(new RawPendingMessage
							{
								Destination = other.ConnectedClient,
								Message = String.Format(Message, args.ToArray())
							});
						}
					}
					break;
			}

            DatabaseLock.ReleaseMutex();
        }

        public static void CommandProcessingThread()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(10);

				while (PendingActions.Count > 0)
				{
					CommandLock.WaitOne();
					var PendingCommand = PendingActions.First.Value;
					PendingActions.RemoveFirst();
					CommandLock.ReleaseMutex();

                    DatabaseLock.WaitOne();
                    PendingCommand();
                    SendPendingMessages();
                    DatabaseLock.ReleaseMutex();
                }
            }
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
