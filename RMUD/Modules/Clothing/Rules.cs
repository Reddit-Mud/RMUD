using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.Modules._Clothing
{
    public class ClothingRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
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
                            MudObject.SendMessage(actor, "You'll have to remove <the0> first.", wornItem);
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
                            MudObject.SendMessage(actor, "You'll have to remove <the0> first.", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Allow;
                })
                .Name("Can't remove items under other items rule.");

            GlobalRules.Perform<MudObject, Actor>("describe")
                .First
                .When((viewer, actor) => GlobalRules.ConsiderValueRule<bool>("actor knows actor?", viewer, actor))
                .Do((viewer, actor) =>
                {
                    MudObject.SendMessage(viewer, "^<the0>, a " + (actor.Gender == Gender.Male ? "man." : "woman."), actor);
                    return PerformResult.Continue;
                })
                .Name("Report gender of known actors rule.");

            GlobalRules.Perform<MudObject, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var wornItems = new List<Clothing>(actor.EnumerateObjects<Clothing>(RelativeLocations.Worn));
                    if (wornItems.Count == 0)
                        MudObject.SendMessage(viewer, "^<the0> is naked.", actor);
                    else
                        MudObject.SendMessage(viewer, "^<the0> is wearing " + String.Join(", ", wornItems.Select(c => c.Indefinite(viewer))) + ".", actor);

                    var heldItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Held));
                    if (heldItems.Count == 0)
                        MudObject.SendMessage(viewer, "^<the0> is empty handed.", actor);
                    else
                        MudObject.SendMessage(viewer, "^<the0> is holding " + String.Join(", ", heldItems.Select(i => i.Indefinite(viewer))) + ".", actor);

                    return PerformResult.Continue;
                })
                .Name("List worn and held items when describing an actor rule.");
        }
    }
}
