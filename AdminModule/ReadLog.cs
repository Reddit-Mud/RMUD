using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AdminModule
{
    internal class ReadLog : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("!LOG"),
                    Path("FILENAME"),
                    Optional(Number("COUNT"))))
                .Manual("Displays the last COUNT lines of a log file. If no count is provided, 20 lines are displayed.")
                .ProceduralRule((match, actor) =>
                {
                    int count = 20;
                    if (match.ContainsKey("COUNT")) count = (match["COUNT"] as int?).Value;
                    var filename = match["FILENAME"].ToString() + ".log";
                    if (System.IO.File.Exists(filename))
                    {
                        foreach (var line in new ReverseLineReader(filename).Take(count).Reverse())
                            MudObject.SendMessage(actor, line);
                    }
                    else
                        MudObject.SendMessage(actor, "I could not find that log file.");
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}