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

            if (Core.Start(commandLineOptions.DATABASEPATH))
            {
                telnetListener = new TelnetClientSource();
                telnetListener.Port = Core.SettingsObject.TelnetPort;
                telnetListener.Listen();
            }

                while (true) 
                { 
                    //Todo: Shutdown server command breaks this loop.
                }
            
            telnetListener.Shutdown();
            Core.Shutdown();
        }
    }
}
