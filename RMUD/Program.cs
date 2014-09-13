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
			var mudCore = new MudCore();
			if (mudCore.Start("database/"))
			{
				var telnetListener = new TelnetClientSource();
				telnetListener.Listen(mudCore);
			}
		}
    }
}
