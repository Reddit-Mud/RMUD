using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.StandardActions
{
    internal class Inventory : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Or(
                    KeyWord("INVENTORY"),
                    KeyWord("INV"),
                    KeyWord("I")))
                .Manual("Displays what you are wearing and carrying.")
                .Perform("inventory", "ACTOR");
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject>("inventory", "[Actor] : Describes a player's inventory to themselves.");

            GlobalRules.Perform<MudObject>("inventory")
                .When(a => !(a is Actor))
                .Do(a => PerformResult.Stop)
                .Name("Don't try to list inventory for something that isn't an actor rule.");

            GlobalRules.Perform<MudObject>("inventory")
                .Do(a =>
                {
                    var wornObjects = (a as Actor).GetContents(RelativeLocations.Worn);
                    if (wornObjects.Count == 0) MudObject.SendMessage(a, "You are naked.");
                    else
                    {
                        MudObject.SendMessage(a, "You are wearing..");
                        foreach (var item in wornObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List worn items in inventory rule.");

            GlobalRules.Perform<MudObject>("inventory")
                .Do(a =>
                {
                    var heldObjects = (a as Actor).GetContents(RelativeLocations.Held);
                    if (heldObjects.Count == 0) MudObject.SendMessage(a, "You have nothing.");
                    else
                    {
                        MudObject.SendMessage(a, "You are carrying..");
                        foreach (var item in heldObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List held items in inventory rule.");
        }
    }
}
