using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Admin
{
    internal class DumpRuleDefs : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!DUMPRULES")))
                .Manual("Dumps all defined global rule books to a file.")
                .ProceduralRule((match, actor) =>
                {
                    MudObjectTransformer.StandardRules.DumpStandardRuleArgumentTypes("ruledefs.txt");
                    return PerformResult.Continue;
                });
        }

        
    }
}