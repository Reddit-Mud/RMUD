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
			TelnetClientSource telnetListener = null;

			if (Mud.Start("database/"))
			{
				telnetListener = new TelnetClientSource();
				telnetListener.Listen();
			}

			while (true)
			{
				var command = Console.ReadLine();

				if (command.ToUpper() == "STOP")
					break;
				else if (command.ToUpper() == "CLIENTS")
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
			}

			telnetListener.Shutdown();
			Mud.Shutdown();
		}
    }
}
