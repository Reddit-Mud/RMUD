using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class ClothingRules : DeclaresRules
    {
        public void InitializeRules()
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
        }
    }

    public enum ClothingLayer
    {
        Under = 0,
        Outer = 1,
        Assecories = 2,
        Over = 3
    }

    public enum ClothingBodyPart
    {
        Feet,
        Legs,
        Torso,
        Hands,
        Neck,
        Head,
        Wrist,
        Fingers,
        Ears,
        Face,
        Cloak,
    }

    public class Clothing : MudObject
    {
        public ClothingLayer Layer = ClothingLayer.Outer;
        public ClothingBodyPart BodyPart = ClothingBodyPart.Torso;

        public Clothing() : base() { }
        public Clothing(String Short, String Long) : base(Short, Long) { }

        public static Clothing Create(String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            var r = new Clothing(Short, "This is a generic " + Short + ". Layer: " + Layer + " BodyPart: " + BodyPart);
            r.Layer = Layer;
            r.BodyPart = BodyPart;
            return r;
        }
    }
}
