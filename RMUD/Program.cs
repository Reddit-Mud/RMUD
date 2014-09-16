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
		static void Main(string[] args)
		{
			if (Mud.Start("database/"))
			{
				var telnetListener = new TelnetClientSource();
				telnetListener.Listen();
			}
		}
    }
}
