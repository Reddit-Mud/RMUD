using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Conversation : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("GREET"),
                        new KeyWord("HELLO")),
                    new FailIfNoMatches(
                        new ObjectMatcher("LOCUTOR", new InScopeObjectSource(), (actor, mudobject) =>
                            {
                                if (mudobject is NPC) return MatchPreference.VeryLikely;
                                else return MatchPreference.VeryUnlikely;
                            }),
                        "Whom do you want to greet?")),
                null,
                "Enter into conversation with an NPC.");


        }
    }

}
