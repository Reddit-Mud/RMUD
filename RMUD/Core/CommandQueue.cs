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

        internal static ParserCommandHandler ParserCommandHandler;
        public static CommandParser Parser { get { return ParserCommandHandler.Parser; } }
        internal static LoginCommandHandler LoginCommandHandler;

        internal static void EnqueuClientCommand(Client Client, String RawCommand)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Client = Client, RawCommand = RawCommand });
            PendingCommandLock.ReleaseMutex();
        }

        private static void InitializeCommandProcessor()
        {
            ParserCommandHandler = new ParserCommandHandler();
            LoginCommandHandler = new LoginCommandHandler();

            CommandExecutionThread = new Thread(ProcessCommands);
            CommandExecutionThread.Start();
        }
                
        private static void ProcessCommands()
        {
            while (!ShuttingDown)
            {
                System.Threading.Thread.Sleep(10);
                Heartbeat();

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
                        LogCommandError(e);
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
                            LogCommandError(e);
                            ClearPendingMessages();
                        }

                        DatabaseLock.ReleaseMutex();
                    }
                }
            }
        }
    }
}
