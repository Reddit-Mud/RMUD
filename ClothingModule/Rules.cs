using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace ClothingModule
{
    public class ClothingRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
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

            GlobalRules.Value<MudObject, bool>("wearable?")
                .When(a => a is Clothing)
                .Do(a => true)
                .Name("Clothing is wearable rule.");

            GlobalRules.Check<MudObject, MudObject>("can wear?")
                .When((actor, item) => item is Clothing && actor is Actor)
                .Do((actor, item) =>
                {
                    var article = item as Clothing;
                    foreach (var wornItem in (actor as Actor).EnumerateObjects<Clothing>(RelativeLocations.Worn))
                        if (wornItem.BodyPart == article.BodyPart && article.Layer <= wornItem.Layer)
                        {
                            MudObject.SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Continue;
                })
                .Name("Check clothing layering before wearing rule.");

            GlobalRules.Check<MudObject, MudObject>("can remove?")
                .When((actor, item) => actor is Actor && item is Clothing)
                .Do((actor, item) =>
                {
                    var article = item as Clothing;
                    foreach (var wornItem in (actor as Actor).EnumerateObjects<Clothing>(RelativeLocations.Worn))
                        if (wornItem.BodyPart == article.BodyPart && article.Layer < wornItem.Layer)
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
                    var wornItems = new List<Clothing>(actor.EnumerateObjects<Clothing>(RelativeLocations.Worn));
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
