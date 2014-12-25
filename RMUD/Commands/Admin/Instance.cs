using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Instance : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!INSTANCE"),
                    MustMatch("It helps if you give me a path.",
                        Path("PATH"))),
                "Create a new instance of an object.")
                .Manual("Given a path, create a new instance of an object.")
                .ProceduralRule((match, actor) =>
                {
                    var path = match.Arguments["PATH"].ToString();
                    var newObject = Mud.CreateInstance(path + "@" + Guid.NewGuid().ToString(), s => Mud.SendMessage(actor, s));
                    if (newObject == null) Mud.SendMessage(actor, "Failed to instance " + path + ".");
                    else
                    {
                        MudObject.Move(newObject, actor);
                        Mud.SendMessage(actor, "Instanced " + path + ".");
                    }
                    return PerformResult.Continue;
                });
        }
    }
}