﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using NetworkModule.Telnet;
using RMUD;

namespace Delmud
{
    class Program
    {
        static void Main(string[] args)
        {
            TelnetClientSource telnetListener = null;

            if (Core.Start(StartupFlags.SearchDirectory, "delmud_db/", new RuntimeDatabase()))
            {
                telnetListener = new TelnetClientSource();
                telnetListener.Port = Core.SettingsObject.TelnetPort;
                telnetListener.Listen();

                while (!Core.ShuttingDown)
                {
                    //Todo: Shutdown server command breaks this loop.
                }

                telnetListener.Shutdown();
            }
            else
            {
                while (true) { }
            }
        }
    }
}
