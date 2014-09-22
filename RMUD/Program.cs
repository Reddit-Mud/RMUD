﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Database.Server.Responders;

namespace RMUD
{
    class Program
    {
		internal class CommandLineOptions
		{
			public int PORT { get; set; }
			public String DATABASEPATH { get; set; }

			public CommandLineOptions()
			{
				PORT = 8669;
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

			if (Mud.Start(commandLineOptions.DATABASEPATH))
			{
				telnetListener = new TelnetClientSource();
				telnetListener.Port = commandLineOptions.PORT;
				telnetListener.Listen();
			}

			while (true)
			{
				var command = Console.ReadLine().ToUpper();

			    if (!String.IsNullOrWhiteSpace(command))
			    {
			        Console.WriteLine(command); // Echo


			        if (command == "STOP")
			            break;
			        else if (command == "CLIENTS")
			        {
			            Mud.DatabaseLock.WaitOne();

			            foreach (var client in Mud.ConnectedClients)
			            {
			                Console.Write(client.ConnectionDescription);
			                if (client.Player != null)
			                {
			                    Console.Write(" -- ");
			                    Console.Write(client.Player.Short);
			                }
			                Console.WriteLine();
			            }

			            Mud.DatabaseLock.ReleaseMutex();
			        }
			        else if (command == "MEMORY")
			        {
			            var mem = System.GC.GetTotalMemory(false);
			            var kb = mem/1024.0f;
			            Console.WriteLine("Memory usage: " + String.Format("{0:n0}", kb) + " kb");
			            Console.WriteLine("Named objects loaded: " + Mud.NamedObjects.Count);
			        }
			        else if (command == "SAVE")
			        {
			            Console.WriteLine("Saving {0} NamedObjects", Mud.NamedObjects.Count);
			            Mud.DatabaseLock.WaitOne();
			            using (var session = Mud.PersistentStore.BulkInsert())
			            {
			                foreach (var obj in Mud.NamedObjects)
			                {
			                    Console.WriteLine("Saving {0}", obj.Value.Id);
			                    session.Store(obj.Value);
			                }
			            }
			            Mud.DatabaseLock.ReleaseMutex();
			        }
			    }

			}

			telnetListener.Shutdown();
			Mud.Shutdown();
		}
    }
}
