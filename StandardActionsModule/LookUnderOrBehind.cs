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
                .ID("StandardActions:LookRelloc")
                .Manual("Lists object that are in, on, under, or behind the object specified.")
                .Check("can look relloc?", "ACTOR", "OBJECT", "RELLOC")
                .Perform("look relloc", "ACTOR", "OBJECT", "RELLOC");
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("cant look relloc", "You can't look <s0> that.");
            Core.StandardMessage("is closed error", "^<the0> is closed.");
            Core.StandardMessage("relloc it is", "^<s0> <the1> is..");
            Core.StandardMessage("nothing relloc it", "There is nothing <s0> <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, RelativeLocations>("can look relloc?", "[Actor, Item, Relative Location] : Can the actor look in/on/under/behind the item?", "actor", "item", "relloc");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Container must be visible rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => (item.LocationsSupported & relloc) != relloc)
                .Do((actor, item, relloc) =>
                {
                    MudObject.SendMessage(actor, "@cant look relloc", Relloc.GetRelativeLocationName(relloc));
                    return SharpRuleEngine.CheckResult.Disallow;
                })
                .Name("Container must support relloc rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => (relloc == RelativeLocations.In) && !item.GetPropertyOrDefault<bool>("open?", false))
                .Do((actor, item, relloc) =>
                {
                        MudObject.SendMessage(actor, "@is closed error", item);
                        return SharpRuleEngine.CheckResult.Disallow;
                })
                .Name("Container must be open to look in rule.");

            GlobalRules.Check<MudObject, MudObject, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => SharpRuleEngine.CheckResult.Allow)
                .Name("Default allow looking relloc rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, RelativeLocations>("look relloc", "[Actor, Item, Relative Location] : Handle the actor looking on/under/in/behind the item.", "actor", "item", "relloc");

            GlobalRules.Perform<MudObject, MudObject, RelativeLocations>("look relloc")
                .Do((actor, item, relloc) =>
                {
                    var contents = new List<MudObject>(item.EnumerateObjects(relloc));

                    if (contents.Count > 0)
                    {
                        MudObject.SendMessage(actor, "@relloc it is", Relloc.GetRelativeLocationName(relloc), item);
                        foreach (var thing in contents)
                            MudObject.SendMessage(actor, "  <a0>", thing);
                    }
                    else
                        MudObject.SendMessage(actor, "@nothing relloc it", Relloc.GetRelativeLocationName(relloc), item);

                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("List contents in relative location rule.");
        }
    }
}