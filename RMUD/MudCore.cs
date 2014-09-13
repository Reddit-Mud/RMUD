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
		private static CommandParser Parser = new CommandParser();
		internal static List<Message> PendingMessages = new List<Message>();
		
        internal static void EnqueuAction(Action Action)
        {
            CommandLock.WaitOne();
            PendingActions.AddLast(Action);
            CommandLock.ReleaseMutex();
        }

        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
			EnqueuAction(() => { HandleClientCommand(Client, RawCommand); });
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
			client.Player.Location = "dummy";
			client.Player.ConnectedClient = client;
			DatabaseLock.WaitOne();
            ConnectedClients.Add(client);
			DatabaseLock.ReleaseMutex();
        }

        public static bool Start(String basePath)
        {
			//Iterate over all types, find ICommandFactories, Create commands
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsSubclassOf(typeof(CommandFactory)))
				{
					var instance = Activator.CreateInstance(type) as CommandFactory;
					instance.Create(Parser);
				}
			}

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

		internal static void HandleClientCommand(Client Executor, String RawCommand)
        {
            try
			{
				var matchedCommand = Parser.ParseCommand(RawCommand, Executor.Player);
				if (matchedCommand != null)
					matchedCommand.Command.Processor.Perform(matchedCommand.Match, Executor.Player);
				else
					SendMessage(Executor, "huh?", true);
			}
			catch (Exception e)
			{
				SendMessage(Executor, e.Message, true);
			}
        }
    }
}
