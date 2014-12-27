using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Move : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!MOVE"),
                    MustMatch("I don't see that here.",
                        Object("OBJECT", InScope)),
                    OptionalKeyWord("TO"),
                    MustMatch("You have to specify the path of the destination.",
                        Path("DESTINATION"))))
                .Manual("An administrative command to move objects from one place to another. This command entirely ignores all rules that might prevent moving an object.")
                .ProceduralRule((match, actor) =>
                {
                    var destination = Mud.GetObject(match["DESTINATION"].ToString());
                    if (destination != null)
                    {
                        var target = match["OBJECT"] as MudObject;
                        Mud.MarkLocaleForUpdate(target);
                        MudObject.Move(target, destination);
                        Mud.MarkLocaleForUpdate(destination);

                        Mud.SendMessage(actor, "Success.");
                    }
                    else
                        Mud.SendMessage(actor, "I could not find the destination.");
                    return PerformResult.Continue;
                });
        }
    }
}