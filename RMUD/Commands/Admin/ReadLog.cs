using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class ReadLog : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("!LOG"),
                    Path("FILENAME"),
                    Optional(Number("COUNT"))),
                "Display a log file.")
                .Manual("Displays the last COUNT lines of a log file. If no count is provided, 20 lines are displayed.")
                .ProceduralRule((match, actor) =>
                {
                    int count = 20;
                    if (match.Arguments.ContainsKey("COUNT")) count = (match.Arguments["COUNT"] as int?).Value;
                    var filename = match.Arguments["FILENAME"].ToString() + ".log";
                    if (System.IO.File.Exists(filename))
                    {
                        foreach (var line in new ReverseLineReader(filename).Take(count).Reverse())
                            Mud.SendMessage(actor, line);
                    }
                    else
                        Mud.SendMessage(actor, "I could not find that log gile.");
                    return PerformResult.Continue;
                });
        }
    }
}