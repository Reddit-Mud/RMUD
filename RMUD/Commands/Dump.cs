using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Dump : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("DUMP"),
                    MustMatch("It helps if you supply a path.",
                        Path("TARGET"))),
                "Dump a database source file.")
                .Manual("Display the source of a database object.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match.Arguments["TARGET"].ToString();
                    var source = Mud.LoadSourceFile(target);
                    if (!source.Item1)
                        Mud.SendMessage(actor, "Could not display source: " + source.Item2);
                    else
                        Mud.SendMessage(actor, "Source of " + target + "\n" + source.Item2);
                    return PerformResult.Continue;
                });
        }
    }
}