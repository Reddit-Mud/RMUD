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
                Sequence(
                    RequiredRank(500),
                    KeyWord("!STATS"),
                    Optional(SingleWord("TYPE"))))
                .Manual("Displays various stats about the server. Type the command with no argument to get a list of available options.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.Arguments.ContainsKey("TYPE"))
                        Mud.SendMessage(actor, "Try one of these options: CLIENTS MEMORY HEARTBEAT TIME");
                    else
                    {
                        var type = match.Arguments["TYPE"].ToString().ToUpper();

                        if (type == "CLIENTS")
                        {
                            Mud.SendMessage(actor, "~~ CLIENTS ~~");
                            foreach (var client in Mud.ConnectedClients)
                                Mud.SendMessage(actor, client.ConnectionDescription + (client.Player == null ? "" : (" - " + client.Player.Short)));
                        }
                        else if (type == "MEMORY")
                        {
                            var mem = System.GC.GetTotalMemory(false);
                            var kb = mem / 1024.0f;
                            Mud.SendMessage(actor, "Memory usage: " + String.Format("{0:n0}", kb) + " kb");
                            Mud.SendMessage(actor, "Named objects loaded: " + Mud.NamedObjects.Count);
                        }
                        else if (type == "TIME")
                        {
                            Mud.SendMessage(actor, String.Format("Current time in game: {0}\r\n", Mud.TimeOfDay));
                            Mud.SendMessage(actor, String.Format("Advance rate: {0} per heartbeat\r\n", Mud.SettingsObject.ClockAdvanceRate));
                        }
                        else
                            Mud.SendMessage(actor, "That isn't an option I understand.");
                    }
                    return PerformResult.Continue;

                });
        }
    }
}