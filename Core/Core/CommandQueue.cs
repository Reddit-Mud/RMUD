﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class PendingCommand
    {
        public MudObject Actor;
        public String RawCommand;
        internal Action ProcessingCompleteCallback;
        internal Dictionary<String, Object> PreSettings;
    }

    public static partial class Core
    {
        private static Mutex PendingCommandLock = new Mutex();
        private static LinkedList<PendingCommand> PendingCommands = new LinkedList<PendingCommand>();
        private static Thread CommandExecutionThread;
        private static Thread IndividualCommandThread;
        private static AutoResetEvent CommandReadyHandle = new AutoResetEvent(false);
        private static AutoResetEvent CommandFinishedHandle = new AutoResetEvent(false);
        private static PendingCommand NextCommand;

        //The client command handler can set this flag when it wants the command timeout to be ignored.
        public static bool CommandTimeoutEnabled = true;


        public static ParserCommandHandler ParserCommandHandler;
        public static CommandParser DefaultParser;

        public static void EnqueuActorCommand(MudObject Actor, String RawCommand, Dictionary<String, Object> MatchPreSettings = null)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, PreSettings = MatchPreSettings });
            PendingCommandLock.ReleaseMutex();
        }

        public static void EnqueuActorCommand(MudObject Actor, String RawCommand, Action ProcessingCompleteCallback)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, ProcessingCompleteCallback = ProcessingCompleteCallback });
            PendingCommandLock.ReleaseMutex();
        }

        public static void EnqueuActorCommand(PendingCommand Command)
        {
            PendingCommandLock.WaitOne();
            PendingCommands.AddLast(Command);
            PendingCommandLock.ReleaseMutex();
        }

        internal static void DiscoverCommandFactories(ModuleAssembly In, CommandParser AddTo)
        {
            foreach (var type in In.Assembly.GetTypes())
            {
                AddTo.ModuleBeingInitialized = In.FileName;
                if (type.FullName.StartsWith(In.Info.BaseNameSpace) && type.IsSubclassOf(typeof(CommandFactory)))
                    CommandFactory.CreateCommandFactory(type).Create(AddTo);
            }
        }

        private static void InitializeCommandProcessor()
        {
            foreach (var assembly in IntegratedModules)
                DiscoverCommandFactories(assembly, DefaultParser);

            ParserCommandHandler = new ParserCommandHandler();
        }

        private static void StartThreadedCommandProcesor()
        {
            CommandExecutionThread = new Thread(ProcessThreadedCommands);
            CommandExecutionThread.Start();
        }

        private static void ProcessCommandsWorkerThread()
        {
            while (!ShuttingDown)
            {
                CommandReadyHandle.WaitOne();

                try
                {
                    //if (NextCommand.Actor.ConnectedClient != null)
                    //    NextCommand.Actor.ConnectedClient.TimeOfLastCommand = DateTime.Now;
                    var commandHandler = NextCommand.Actor.GetProperty<ClientCommandHandler>("command handler");
                    if (commandHandler != null)
                        commandHandler.HandleCommand(NextCommand);
                }
                catch (System.Threading.ThreadAbortException)
                {
                    LogError("Command worker thread was aborted. Timeout hit?");
                    Core.ClearPendingMessages();
                }
                catch (Exception e)
                {
                    LogCommandError(e);
                    Core.ClearPendingMessages();
                }

                NextCommand = null;

                CommandFinishedHandle.Set();
            }
        }

        private static void ProcessThreadedCommands()
        {
            if ((Core.Flags & StartupFlags.SingleThreaded) == StartupFlags.SingleThreaded)
                throw new InvalidOperationException("ProcessThreadedCommands should never be called in single threaded mode.");

            IndividualCommandThread = new Thread(ProcessCommandsWorkerThread);
            IndividualCommandThread.Start();

            while (!ShuttingDown)
            {
                System.Threading.Thread.Sleep(10);
                DatabaseLock.WaitOne();
                Heartbeat();
                DatabaseLock.ReleaseMutex();

                while (PendingCommands.Count > 0 && !ShuttingDown)
                {
                    PendingCommand PendingCommand = null;

                    PendingCommandLock.WaitOne();

                    try
                    {
                        PendingCommand = PendingCommands.FirstOrDefault(pc =>
                            {
                                return true;
                                //if (pc.Actor.ConnectedClient == null) return true;
                                //else return (DateTime.Now - pc.Actor.ConnectedClient.TimeOfLastCommand).TotalMilliseconds > SettingsObject.AllowedCommandRate;
                            });
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

                        NextCommand = PendingCommand;

                        //Reset flags that the last command may have changed
                        CommandTimeoutEnabled = true;
                        SilentFlag = false;
                        GlobalRules.LogRules(null);

                        CommandReadyHandle.Set(); //Signal worker thread to proceed.
                        if (!CommandFinishedHandle.WaitOne(SettingsObject.CommandTimeOut))
                        {
                            if (!CommandTimeoutEnabled) //Timeout is disabled, go ahead and wait for infinity.
                                CommandFinishedHandle.WaitOne();
                            else
                            {
                                //Kill the command processor thread.
                                IndividualCommandThread.Abort();
                                ClearPendingMessages();
                                var client = PendingCommand.Actor.GetProperty<Client>("client");
                                if (client != null)
                                {
                                    client.Send("Command timeout.\r\n");
                                    LogError(String.Format("Command timeout. {0} - {1}", client.ConnectionDescription, PendingCommand.RawCommand));
                                }
                                else
                                    LogError(String.Format("Command timeout [No client] - {0}", PendingCommand.RawCommand));
                                IndividualCommandThread = new Thread(ProcessCommandsWorkerThread);
                                IndividualCommandThread.Start();
                            }
                        }

                        if (PendingCommand.ProcessingCompleteCallback != null)
                            PendingCommand.ProcessingCompleteCallback();

                        DatabaseLock.ReleaseMutex();
                    }
                }
            }

            IndividualCommandThread.Abort();
            if (Core.OnShutDown != null) Core.OnShutDown();
        }

        /// <summary>
        /// Process commands in a single thread, until there are no more queued commands.
        /// Heartbeat is called between every command.
        /// </summary>
        public static void ProcessCommands()
        {
            if ((Core.Flags & StartupFlags.SingleThreaded) != StartupFlags.SingleThreaded)
                throw new InvalidOperationException("ProcessCommands should only be called in single threaded mode.");

            while (PendingCommands.Count > 0 && !ShuttingDown)
            {
                GlobalRules.ConsiderPerformRule("heartbeat");

                Core.SendPendingMessages();
               
                PendingCommand PendingCommand = null;

                try
                {
                    PendingCommand = PendingCommands.FirstOrDefault();
                    if (PendingCommand != null)
                        PendingCommands.Remove(PendingCommand);
                }
                catch (Exception e)
                {
                    LogCommandError(e);
                    PendingCommand = null;
                }

                if (PendingCommand != null)
                {
                    NextCommand = PendingCommand;

                    //Reset flags that the last command may have changed
                    CommandTimeoutEnabled = true;
                    SilentFlag = false;
                    GlobalRules.LogRules(null);

                    try
                    {
                        var handler = NextCommand.Actor.GetProperty<ClientCommandHandler>("command handler");
                        if (handler != null)
                            handler.HandleCommand(NextCommand);
                    }
                    catch (Exception e)
                    {
                        LogCommandError(e);
                        Core.ClearPendingMessages();
                    }
                    if (PendingCommand.ProcessingCompleteCallback != null)
                        PendingCommand.ProcessingCompleteCallback();

                }
            }
        }
    }
}
