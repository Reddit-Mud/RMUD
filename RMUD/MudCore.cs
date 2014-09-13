using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RMUD
{
    public partial class MudCore
    {
        Mutex CommandLock = new Mutex();
        LinkedList<Action> PendingActions = new LinkedList<Action>();
        Thread ActionExecutionThread;
        public Database Database { get; private set; }
        internal List<Client> ConnectedClients = new List<Client>();
        internal Mutex DatabaseLock = new Mutex();

        public MudCore()
        {
        }

        internal void EnqueuAction(Action action)
        {
            CommandLock.WaitOne();
            PendingActions.AddLast(action);
            CommandLock.ReleaseMutex();
        }

        public void EnqueuClientCommand(Client Client, String RawCommand)
        {
            EnqueuAction(new ClientCommand(Client, RawCommand));
        }

        public void ClientDisconnected(Client client)
        {
            //Console.WriteLine("Lost client " + (client.logged_on ? client.player.GetProperty("@path").ToString() : "null") + "\n");
            if (client.logged_on)
            {
                DatabaseLock.WaitOne();
                ConnectedClients.Remove(client);
                DatabaseLock.ReleaseMutex();
            }
            //EnqueuAction(new InvokeSystemAction(client, "handle-lost-client", 0.0f));
        }

        public void ClientConnected(Client client)
        {
            //EnqueuAction(new InvokeSystemAction(client, "handle-new-client", 0.0f));
            ConnectedClients.Add(client);
        }

        public bool Start(String basePath)
        {
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

        public void Join()
        {
            ActionExecutionThread.Join();
        }

        internal class Message
        {
            public Client Client;
            public String Data;
        }

        internal List<Message> PendingMessages = new List<Message>();

        public void SendMessage(Client Client, String Data, bool Immediate)
        {
            DatabaseLock.WaitOne();
            if (Immediate) Client.Send(Data);
            else PendingMessages.Add(new Message { Client = Client, Data = Data });
            DatabaseLock.ReleaseMutex();
        }

        public void CommandProcessingThread()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(10);

				while (PendingActions.Count > 0)
				{
					var PendingCommand = PendingActions.First.Value;
					PendingActions.RemoveFirst();

                    DatabaseLock.WaitOne();
                    PendingCommand.Execute(this);
                    SendPendingMessages();
                    DatabaseLock.ReleaseMutex();
                }
            }
        }

        internal void SendPendingMessages()
        {
            foreach (var Message in PendingMessages)
                Message.Client.Send(Message.Data);
            PendingMessages.Clear();
        }

    }
}
