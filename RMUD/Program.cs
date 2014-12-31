using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace RMUD
{
    class Program
    {
		internal class CommandLineOptions
		{
			public String DATABASEPATH { get; set; }

			public CommandLineOptions()
			{
				DATABASEPATH = "database/";
			}
		}

        static void Main(string[] args)
        {
            var commandLineOptions = new CommandLineOptions();
            var error = CommandLine.ParseCommandLine(commandLineOptions);

            if (error != CommandLine.Error.Success)
            {
                Console.WriteLine("Command line error: " + error);
                return;
            }

            TelnetClientSource telnetListener = null;

            if (MudObject.Start(commandLineOptions.DATABASEPATH))
            {
                telnetListener = new TelnetClientSource();
                telnetListener.Port = MudObject.SettingsObject.TelnetPort;
                telnetListener.Listen();
            }

            if (MudObject.SettingsObject.UseConsoleCommands)
            {
                try
                {
                    while (true)
                    {
                        var command = Console.ReadLine();

                        if (command.ToUpper() == "STOP")
                            break;
                        else if (command.ToUpper() == "CLIENTS")
                        {
                            MudObject.DatabaseLock.WaitOne();

                            foreach (var client in MudObject.ConnectedClients)
                            {
                                Console.Write(client.ConnectionDescription);
                                if (client.Player != null)
                                {
                                    Console.Write(" -- ");
                                    Console.Write(client.Player.Short);
                                }
                                Console.WriteLine();
                            }

                            MudObject.DatabaseLock.ReleaseMutex();
                        }
                        else if (command.ToUpper() == "MEMORY")
                        {
                            var mem = System.GC.GetTotalMemory(false);
                            var kb = mem / 1024.0f;
                            Console.WriteLine("Memory usage: " + String.Format("{0:n0}", kb) + " kb");
                            Console.WriteLine("Named objects loaded: " + MudObject.NamedObjects.Count);
                        }
                        else if (command.ToUpper() == "TIME")
                        {
                            MudObject.DatabaseLock.WaitOne();
                            Console.WriteLine("Current time in game: {0}", MudObject.TimeOfDay);
                            Console.WriteLine("Advance rate: {0} per heartbeat",
                                MudObject.SettingsObject.ClockAdvanceRate);
                            MudObject.DatabaseLock.ReleaseMutex();
                        }
                        else if (command.ToUpper() == "SAVE")
                        {
                            MudObject.DatabaseLock.WaitOne();
                            Console.Write("Saving persistent instances to file...");
                            var totalInstances = MudObject.SaveActiveInstances();
                            Console.WriteLine(totalInstances + " instances saved");
                        }
                        else if (command.ToUpper() == "DEBUGON")
                        {
                            MudObject.CommandTimeoutEnabled = false;
                            Console.WriteLine("Debugging mode enabled");
                        }
                        else if (command.ToUpper() == "DEBUGOFF")
                        {
                            MudObject.CommandTimeoutEnabled = true;
                            Console.WriteLine("Debugging mode disabled");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR:", e.Message, e.Source, e.StackTrace, e.Data);
                }

            }
            else
            {
                while (true) 
                { 
                    //Todo: Shutdown server command breaks this loop.
                }
            }

            telnetListener.Shutdown();
            MudObject.Shutdown();
        }
    }
}
