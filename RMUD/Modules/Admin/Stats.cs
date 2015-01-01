using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Admin
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
                    if (!match.ContainsKey("TYPE"))
                        MudObject.SendMessage(actor, "Try one of these options: CLIENTS MEMORY HEARTBEAT TIME");
                    else
                    {
                        var type = match["TYPE"].ToString().ToUpper();

                        if (type == "CLIENTS")
                        {
                            MudObject.SendMessage(actor, "~~ CLIENTS ~~");
                            foreach (var client in Core.ConnectedClients)
                                MudObject.SendMessage(actor, client.ConnectionDescription + (client.Player == null ? "" : (" - " + client.Player.Short)));
                        }
                        else if (type == "MEMORY")
                        {
                            //var mem = System.GC.GetTotalMemory(false);
                            //var kb = mem / 1024.0f;
                            //MudObject.SendMessage(actor, "Memory usage: " + String.Format("{0:n0}", kb) + " kb");
                            //MudObject.SendMessage(actor, "Named objects loaded: " + Core.NamedObjects.Count);
                        }
                        else if (type == "TIME")
                        {
                            MudObject.SendMessage(actor, String.Format("Current time in game: {0}\r\n", MudObject.TimeOfDay));
                            MudObject.SendMessage(actor, String.Format("Advance rate: {0} per heartbeat\r\n", Core.SettingsObject.ClockAdvanceRate));
                        }
                        else
                            MudObject.SendMessage(actor, "That isn't an option I understand.");
                    }
                    return PerformResult.Continue;

                });
        }
    }
}