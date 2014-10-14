using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Stats : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("STATS", false),
                    new Optional(new SingleWord("TYPE"))),
                new StatsProcessor(),
                "View server stats.");
        }
	}

	internal class StatsProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            if (!Match.Arguments.ContainsKey("TYPE"))
                Mud.SendMessage(Actor, "Try one of these options: CLIENTS MEMORY HEARTBEAT TIME\r\n");
            else
            {
                var type = Match.Arguments["TYPE"].ToString().ToUpper();
                var builder = new StringBuilder();

                if (type == "CLIENTS")
                {
                    builder.Append("~~All connected clients~~\r\n");
                    foreach (var client in Mud.ConnectedClients)
                    {
                        builder.Append(client.ConnectionDescription);
                        if (client.Player != null)
                        {
                            builder.Append(" -- ");
                            builder.Append(client.Player.Short);
                        }
                        builder.Append("\r\n");
                    }

                }
                else if (type == "MEMORY")
                {
                    var mem = System.GC.GetTotalMemory(false);
                    var kb = mem / 1024.0f;
                    builder.Append("Memory usage: " + String.Format("{0:n0}", kb) + " kb\r\n");
                    builder.Append("Named objects loaded: " + Mud.NamedObjects.Count + "\r\n");
                }
                else if (type == "HEARTBEAT")
                {
                    builder.AppendFormat("Heartbeat interval: {0} Objects: {1} HID: {2}\r\n",
                        Mud.SettingsObject.HeartbeatInterval,
                        Mud.ObjectsRegisteredForHeartbeat.Count,
                        Mud.HeartbeatID);
                    foreach (var Object in Mud.ObjectsRegisteredForHeartbeat)
                        builder.Append(Object.ToString() + "\r\n");
                }
                else if (type == "TIME")
                {
                    builder.AppendFormat("Current time in game: {0}\r\n", Mud.TimeOfDay);
                    builder.AppendFormat("Advance rate: {0} per heartbeat\r\n",
                        Mud.SettingsObject.ClockAdvanceRate);
                }

                Mud.SendMessage(Actor, builder.ToString());
            }
        }
	}

}
