using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using RMUD.Modules.Network.Telnet;

namespace RMUD
{
    class Program
    {
        static void Main(string[] args)
        {
            TelnetClientSource telnetListener = null;

            if (Core.Start(new GithubDatabase()))
            {
                telnetListener = new TelnetClientSource();
                telnetListener.Port = Core.SettingsObject.TelnetPort;
                telnetListener.Listen();

                while (true)
                {
                    //Todo: Shutdown server command breaks this loop.
                }

                telnetListener.Shutdown();
                Core.Shutdown();
            }
            else
            {
                while (true) { }
            }
        }
    }
}
