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
                        Path("PATH"))))
                .Manual("Given a path, create a new instance of an object.")
                .ProceduralRule((match, actor) =>
                {
                    var path = match["PATH"].ToString();
                    var newObject = Core.CreateInstance(path + "@" + Guid.NewGuid().ToString(), s => MudObject.SendMessage(actor, s));
                    if (newObject == null) MudObject.SendMessage(actor, "Failed to instance " + path + ".");
                    else
                    {
                        MudObject.Move(newObject, actor);
                        MudObject.SendMessage(actor, "Instanced " + path + ".");
                    }
                    return PerformResult.Continue;
                });
        }
    }
}