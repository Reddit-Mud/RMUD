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
        private class PendingCommand
        {
            internal Client Client;
            internal String RawCommand;
        }

        private static Mutex PendingCommandLock = new Mutex();
        private static LinkedList<PendingCommand> PendingCommands = new LinkedList<PendingCommand>();
        private static Thread CommandExecutionThread;

        internal static List<Client> ConnectedClients = new List<Client>();
        internal static Mutex DatabaseLock = new Mutex();
		private static bool ShuttingDown = false;

        private static String CriticalLog = "errors.log";

		internal static ParserCommandHandler ParserCommandHandler;
		public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
		internal static LoginCommandHandler LoginCommandHandler;

        internal static Settings SettingsObject;
        internal static ProscriptionList ProscriptionList;

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

        internal static List<MudObject> ChangedObjects = new List<MudObject>();
		
        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Client = Client, RawCommand = RawCommand });
            PendingCommandLock.ReleaseMutex();
        }

        public static void MarkChangedObject(MudObject Object)
        {
            DatabaseLock.WaitOne();
            if (!ChangedObjects.Contains(Object)) ChangedObjects.Add(Object);
            DatabaseLock.ReleaseMutex();
        }

		public static void ClientDisconnected(Client client)
		{
			DatabaseLock.WaitOne();
            RemoveClientFromAllChannels(client);
			ConnectedClients.Remove(client);
            if (client.Player != null)
            {
                client.Player.ConnectedClient = null;
                Thing.Move(client.Player, null);
            }
			DatabaseLock.ReleaseMutex();
		}

        public enum ClientAcceptanceStatus
        {
            Accepted,
            Rejected,
        }

        public static ClientAcceptanceStatus ClientConnected(Client Client)
        {
            var ban = ProscriptionList.IsBanned(Client.IPString);
            if (ban.Banned)
            {
                LogError("Rejected connection from " + Client.IPString + ". Matched ban " + ban.SourceBan.Glob + " Reason: " + ban.SourceBan.Reason );
                return ClientAcceptanceStatus.Rejected;
            }

            DatabaseLock.WaitOne();

            Client.CommandHandler = LoginCommandHandler;

			var settings = GetObject("settings", s => Mud.SendMessage(Client, s + "\r\n")) as Settings;
			Mud.SendMessage(Client, settings.Banner);
			Mud.SendMessage(Client, settings.MessageOfTheDay);

            ConnectedClients.Add(Client);

            SendPendingMessages();

			DatabaseLock.ReleaseMutex();

            return ClientAcceptanceStatus.Accepted;
        }

        public static bool Start(String basePath)
        {
            try
            {
				InitializeDatabase(basePath);
				var settings = GetObject("settings") as Settings;
				if (settings == null) throw new InvalidProgramException("No settings object is defined in the database!");
                SettingsObject = settings;

                ProscriptionList = new ProscriptionList(settings.ProscriptionList);

				ParserCommandHandler = new ParserCommandHandler();
				LoginCommandHandler = new LoginCommandHandler();
				
				CommandExecutionThread = new Thread(ProcessCommands);
                CommandExecutionThread.Start();

                if (settings.UpfrontCompilation)
                {
                    var databaseObjectFiles = new List<String>();
                    EnumerateDatabase("", true, s => databaseObjectFiles.Add(s));

                    Console.WriteLine("Found {0} database objects to load.", databaseObjectFiles.Count);
                    var start = DateTime.Now;
                    foreach (var objectFile in databaseObjectFiles)
                    {
                        GetObject(objectFile);
                    }

                    HandleChanges();

                    Console.WriteLine("Total compilation in {0}.", DateTime.Now - start);
                }

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

        public static void ProcessCommands()
        {
            while (!ShuttingDown)
            {
                System.Threading.Thread.Sleep(10);

				while (PendingCommands.Count > 0)
				{
                    PendingCommand PendingCommand = null;

					PendingCommandLock.WaitOne();
                    try
                    {
                        PendingCommand = PendingCommands.FirstOrDefault(pc => (DateTime.Now - pc.Client.TimeOfLastCommand).TotalMilliseconds > SettingsObject.AllowedCommandRate);
                        if (PendingCommand != null)
                            PendingCommands.Remove(PendingCommand);
                    }
                    catch (Exception e)
                    {
                        LogCriticalError(e);
                        PendingCommand = null;
                    }
					PendingCommandLock.ReleaseMutex();

                    if (PendingCommand != null)
                    {
                        DatabaseLock.WaitOne();

                        try
                        {
                            PendingCommand.Client.TimeOfLastCommand = DateTime.Now;
                            PendingCommand.Client.CommandHandler.HandleCommand(PendingCommand.Client, PendingCommand.RawCommand);
                            HandleChanges();
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

        internal static void HandleChanges()
        {
            var startCount = ChangedObjects.Count;
            for (int i = 0; i < startCount; ++i)
                ChangedObjects[i].HandleChanges();
            ChangedObjects.RemoveRange(0, startCount);
        }

        internal static void SendPendingMessages()
        {
			foreach (var message in PendingMessages)
				message.Destination.Send(message.Message);
            PendingMessages.Clear();
        }

		internal static void ClearPendingMessages()
		{
			PendingMessages.Clear();
		}

    }
}
