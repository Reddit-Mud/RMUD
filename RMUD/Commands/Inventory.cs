using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Inventory : CommandFactory, DeclaresRules
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Or(
                    KeyWord("INVENTORY"),
                    KeyWord("INV"),
                    KeyWord("I")))
                .Manual("Displays what you are wearing and carrying.")
                .Perform("inventory", "ACTOR", "ACTOR");
        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject>("inventory", "[Actor] : Describes a player's inventory to themselves.");

            GlobalRules.Perform<MudObject>("inventory")
                .When(a => !(a is Actor))
                .Do(a => PerformResult.Stop)
                .Name("Don't try to list inventory for something that isn't an actor rule.");

            GlobalRules.Perform<MudObject>("inventory")
                .Do(a =>
                {
                    var wornObjects = (a as Actor).GetContents(RelativeLocations.In);
                    if (wornObjects.Count == 0) Mud.SendMessage(a, "You are naked.");
                    else
                    {
                        Mud.SendMessage(a, "You are wearing..");
                        foreach (var item in wornObjects)
                            Mud.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List worn items in inventory rule.");

            GlobalRules.Perform<MudObject>("inventory")
                .Do(a =>
                {
                    var heldObjects = (a as Actor).GetContents(RelativeLocations.Held);
                    if (heldObjects.Count == 0) Mud.SendMessage(a, "You have nothing.");
                    else
                    {
                        Mud.SendMessage(a, "You are carrying..");
                        foreach (var item in heldObjects)
                            Mud.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List held items in inventory rule.");
        }
    }
}
