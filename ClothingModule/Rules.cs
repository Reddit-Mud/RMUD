using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;
using SharpRuleEngine;

namespace ClothingModule
{
    public class ClothingRules 
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var wornObjects = (a as Actor).GetContents(RelativeLocations.Worn);
                    if (wornObjects.Count == 0) MudObject.SendMessage(a, "@nude");
                    else
                    {
                        MudObject.SendMessage(a, "@clothing wearing");
                        foreach (var item in wornObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List worn items in inventory rule.");

            GlobalRules.Check<Actor, MudObject>("can wear?")
                .Do((actor, item) =>
                {
                    var layer = item.GetPropertyOrDefault<ClothingLayer>("clothing layer", ClothingLayer.Assecories);
                    var part = item.GetPropertyOrDefault<ClothingBodyPart>("clothing part", ClothingBodyPart.Cloak);
                    foreach (var wornItem in actor.EnumerateObjects(RelativeLocations.Worn))
                        if (wornItem.GetPropertyOrDefault<ClothingLayer>("clothing layer", ClothingLayer.Assecories) == layer && wornItem.GetPropertyOrDefault<ClothingBodyPart>("clothing part", ClothingBodyPart.Cloak) == part)
                        {
                            MudObject.SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Continue;
                })
                .Name("Check clothing layering before wearing rule.");

            GlobalRules.Check<Actor, MudObject>("can remove?")
                .Do((actor, item) =>
                {
                    var layer = item.GetPropertyOrDefault<ClothingLayer>("clothing layer", ClothingLayer.Assecories);
                    var part = item.GetPropertyOrDefault<ClothingBodyPart>("clothing part", ClothingBodyPart.Cloak);
                    foreach (var wornItem in actor.EnumerateObjects(RelativeLocations.Worn))
                        if (wornItem.GetPropertyOrDefault<ClothingLayer>("clothing layer", ClothingLayer.Assecories) < layer && wornItem.GetPropertyOrDefault<ClothingBodyPart>("clothing part", ClothingBodyPart.Cloak) == part)
                        {
                            MudObject.SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Allow;
                })
                .Name("Can't remove items under other items rule.");

            
            GlobalRules.Perform<MudObject, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var wornItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Worn));
                    if (wornItems.Count == 0)
                        MudObject.SendMessage(viewer, "@clothing they are naked", actor);
                    else
                        MudObject.SendMessage(viewer, "@clothing they are wearing", actor, wornItems);
                    return PerformResult.Continue;
                })
                .Name("List worn items when describing an actor rule.");
        }
    }
}
