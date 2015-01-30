using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
{
    internal class LookUnderOrBehind : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("LOOK"),
                        KeyWord("L")),
                    RelativeLocation("RELLOC"),
                    Object("OBJECT", InScope)))
                .Manual("Lists object that are in, on, under, or behind the object specified.")
                .Check("can look relloc?", "ACTOR", "OBJECT", "RELLOC")
                .Perform("look relloc", "ACTOR", "OBJECT", "RELLOC");
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, RelativeLocations>("can look relloc?", "[Actor, Item, Relative Location] : Can the actor look in/on/under/behind the item?", "actor", "item", "relloc");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Container must be visible rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => !(item is Container) || (((item as Container).LocationsSupported & relloc) != relloc))
                .Do((actor, item, relloc) =>
                {
                    MudObject.SendMessage(actor, "You can't look " + Relloc.GetRelativeLocationName(relloc) + " that.");
                    return CheckResult.Disallow;
                })
                .Name("Container must support relloc rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => (relloc == RelativeLocations.In) && !GlobalRules.ConsiderValueRule<bool>("open?", item))
                .Do((actor, item, relloc) =>
                {
                        MudObject.SendMessage(actor, "^<the0> is closed.", item);
                        return CheckResult.Disallow;
                })
                .Name("Container must be open to look in rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => CheckResult.Allow)
                .Name("Default allow looking relloc rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, RelativeLocations>("look relloc", "[Actor, Item, Relative Location] : Handle the actor looking on/under/in/behind the item.", "actor", "item", "relloc");

            GlobalRules.Perform<MudObject, MudObject, RelativeLocations>("look relloc")
                .Do((actor, item, relloc) =>
                {
                    var contents = (item as Container).GetContents(relloc);
                    if (contents.Count > 0)
                    {
                        MudObject.SendMessage(actor, "^" + Relloc.GetRelativeLocationName(relloc) + " <the0> is ", item);
                        foreach (var thing in contents)
                            MudObject.SendMessage(actor, "  <a0>", thing);
                    }
                    else
                        MudObject.SendMessage(actor, "There is nothing " + Relloc.GetRelativeLocationName(relloc) + " <the0>.", item);
                    return PerformResult.Continue;
                })
                .Name("List contents in relative location rule.");
        }
    }
}