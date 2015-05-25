using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AdminModule
{
    internal class Scope : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!SCOPE")))
                .Manual("List all of the objects in scope")
                .ProceduralRule((match, actor) =>
                {
                    foreach (var thing in MudObject.EnumerateVisibleTree(MudObject.FindLocale(actor)))
                        MudObject.SendMessage(actor, thing.Short + " - " + thing.GetType().Name);
                    return SharpRuleEngine.PerformResult.Continue;
                }, "List all the damn things in scope rule.");
        }
    }
}
