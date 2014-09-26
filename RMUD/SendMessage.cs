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
        public static void SendMessage(Actor Actor, String Message)
        {
            DatabaseLock.WaitOne();
            if (Actor != null && Actor.ConnectedClient != null)
                PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, Message));
            DatabaseLock.ReleaseMutex();
        }

        public static void SendMessage(Client Client, String Message)
        {
            DatabaseLock.WaitOne();
            PendingMessages.Add(new RawPendingMessage(Client, Message));
            DatabaseLock.ReleaseMutex();
        }

		public static void SendMessage(Actor Actor, MessageScope Scope, String Message)
		{
            DatabaseLock.WaitOne();

			switch (Scope)
			{
                case MessageScope.AllConnectedPlayers:
                    {
                        foreach (var client in ConnectedClients)
                        {
                            if (client.IsLoggedOn)
                                PendingMessages.Add(new RawPendingMessage(client, Message));
                        }
                    }
                    break;

				//Send message only to the player
				case MessageScope.Single:
					{
						if (Actor == null) break;
						if (Actor.ConnectedClient == null) break;
						PendingMessages.Add(new RawPendingMessage(Actor.ConnectedClient, Message));
					}
					break;

				//Send message to everyone in the same location as the player
				case MessageScope.Local:
					{
						if (Actor == null) break;
						var location = Actor.Location as Room;
						if (location == null) break;
						foreach (var thing in location.Contents)
						{
							var other = thing as Actor;
							if (other == null) continue;
							if (other.ConnectedClient == null) continue;
							PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Message));
						}
					}
					break;

				//Send message to everyone in the same location EXCEPT the player.
				case MessageScope.External:
					{
						if (Actor == null) break;
						var location = Actor.Location as Room;
						if (location == null) break;
						foreach (var thing in location.Contents)
						{
							var other = thing as Actor;
							if (other == null) continue;
							if (Object.ReferenceEquals(other, Actor)) continue;
							if (other.ConnectedClient == null) continue;
							PendingMessages.Add(new RawPendingMessage(other.ConnectedClient, Message));
						}
					}
					break;
			}

            DatabaseLock.ReleaseMutex();
        }
    }
}
