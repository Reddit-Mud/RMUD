using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Reload : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!RELOAD"),
                    MustMatch("It helps if you give me a path.",
                        Path("TARGET"))))
                .Manual("Given a path, it attempts to recompile that object. The object will be replaced in-place if possible.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match["TARGET"].ToString();
                    var newObject = MudObject.ReloadObject(target, s => MudObject.SendMessage(actor, s));
                    if (newObject == null) MudObject.SendMessage(actor, "Failed to reload " + target);
                    else MudObject.SendMessage(actor, "Reloaded " + target);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                 Sequence(
                     RequiredRank(500),
                     KeyWord("!RESET"),
                     MustMatch("It helps if you give me a path.",
                         Path("TARGET"))))
                 .Manual("Given a path, it attempts to reset that object without reloading or recompiling. The object will be replaced in-place if possible.")
                 .ProceduralRule((match, actor) =>
                 {
                     var target = match["TARGET"].ToString();
                     if (!MudObject.ResetObject(target, s => MudObject.SendMessage(actor, s))) 
                         MudObject.SendMessage(actor, "Failed to reset " + target);
                     else MudObject.SendMessage(actor, "Reset " + target);
                     return PerformResult.Continue;
                 });

        }
    }
}