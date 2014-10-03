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

        internal static List<MudObject> MarkedObjects = new List<MudObject>();
		
        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Client = Client, RawCommand = RawCommand });
            PendingCommandLock.ReleaseMutex();
        }

        public static Room FindLocale(MudObject Of)
        {
            MudObject locale = Of;

            while (true)
            {
                if (locale == null) return null;
                else if (locale is Room)
                    return locale as Room;
                else if (locale is MudObject)
                {
                    locale = (locale as MudObject).Location;
                    if (Object.ReferenceEquals(locale, Of)) throw new InvalidOperationException("Cycle found in database.");
                }
                else
                    return null; 
            }
        }

        public static MudObject FindVisibilityCeiling(MudObject Of)
        {
            //if (Of is Room) return Of;

            if (Of.Location == null) return Of;

            var container = Of.Location as Container;
            if (container != null)
            {
                var relloc = container.LocationOf(Of);
                if (relloc == RelativeLocations.In) //Consider the openable rules.
                {
                    if (IsOpen(Of.Location)) return FindVisibilityCeiling(Of.Location);
                    else return Of.Location;
                }
            }

            return FindVisibilityCeiling(Of.Location);

        }

        public static bool ObjectContainsObject(MudObject Super, MudObject Sub)
        {
            if (Object.ReferenceEquals(Super, Sub)) return false; //Objects can't contain themselves...
            if (Sub is MudObject)
            {
                var location = (Sub as MudObject).Location;
                if (location == null) return false;
                if (Object.ReferenceEquals(Super, location)) return true;
                return ObjectContainsObject(Super, location);
            }
            return false;    
        }

        public static void MarkLocaleForUpdate(MudObject Object)
        {
            DatabaseLock.WaitOne();
            MudObject locale = FindLocale(Object);
            if (locale != null && !MarkedObjects.Contains(locale))
                MarkedObjects.Add(locale);
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
                MudObject.Move(client.Player, null);
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

                    UpdateMarkedObjects();

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
                            UpdateMarkedObjects();
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

        internal static void UpdateMarkedObjects()
        {
            var startCount = MarkedObjects.Count;
            for (int i = 0; i < startCount; ++i)
                MarkedObjects[i].HandleMarkedUpdate();
            MarkedObjects.RemoveRange(0, startCount);
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
