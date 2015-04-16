using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
    internal class Examine : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    Or(
                        Or(KeyWord("EXAMINE"), KeyWord("X")),
                        Sequence(
                            Or(KeyWord("LOOK"), KeyWord("L")),
                            KeyWord("AT"))),
                    MustMatch("@dont see that", Object("OBJECT", InScope))))
                .Manual("Take a close look at an object.")
                .Check("can examine?", "ACTOR", "OBJECT")
                .Perform("describe", "ACTOR", "OBJECT");
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("dont see that", "I don't see that here.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can examine?", "[Actor, Item] : Can the viewer examine the item?", "actor", "item");

            GlobalRules.Check<MudObject, MudObject>("can examine?")
                .First
                .Do((viewer, item) => MudObject.CheckIsVisibleTo(viewer, item))
                .Name("Can't examine what isn't here rule.");

            GlobalRules.Check<MudObject, MudObject>("can examine?")
                .Last
                .Do((viewer, item) => CheckResult.Allow)
                .Name("Default can examine everything rule.");
        }
    }
}
