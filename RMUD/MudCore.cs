using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static class MudCore
    {
        private static Mutex CommandLock = new Mutex();
        private static LinkedList<Action> PendingActions = new LinkedList<Action>();
        private static Thread ActionExecutionThread;
        public static Database Database { get; private set; }
        private static List<Client> ConnectedClients = new List<Client>();
        private static Mutex DatabaseLock = new Mutex();

		internal static ParserCommandHandler ParserCommandHandler;
		public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
		internal static LoginCommandHandler LoginCommandHandler;

		internal static List<Message> PendingMessages = new List<Message>();
		
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
			DatabaseLock.WaitOne();
            ConnectedClients.Add(client);
			DatabaseLock.ReleaseMutex();
        }

        public static bool Start(String basePath)
        {
			ParserCommandHandler = new ParserCommandHandler();
			LoginCommandHandler = new LoginCommandHandler();

            try
            {
                Database = new Database(basePath);

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

        internal class Message
        {
            public Client Client;
            public String Data;
        }


        public static void SendMessage(Client Client, String Data, bool Immediate = false)
        {
            DatabaseLock.WaitOne();
            if (Immediate) Client.Send(Data);
            else PendingMessages.Add(new Message { Client = Client, Data = Data });
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
            foreach (var Message in PendingMessages)
                Message.Client.Send(Message.Data);
            PendingMessages.Clear();
        }

    }
}
