using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace StandardActionsModule
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
                .ID("StandardActions:Inventory")
                .Manual("Displays what you are wearing and carrying.")
                .Perform("inventory", "ACTOR");
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("empty handed", "You have nothing.");
            Core.StandardMessage("carrying", "You are carrying..");

            GlobalRules.DeclarePerformRuleBook<MudObject>("inventory", "[Actor] : Describes a player's inventory to themselves.", "actor");

            GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var heldObjects = (a as Actor).GetContents(RelativeLocations.Held);
                    if (heldObjects.Count == 0) MudObject.SendMessage(a, "@empty handed");
                    else
                    {
                        MudObject.SendMessage(a, "@carrying");
                        foreach (var item in heldObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("List held items in inventory rule.");
        }
    }
}
