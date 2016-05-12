using RMUD;
using StandardActionsModule;

namespace Akko.Areas.Prologue
{
    public class Car : RMUD.Room
    {
        public override void Initialize()
        {
            Short = "Mr. Henrico's Car";
            
            this.PerformDescribe().FirstTimeOnly.DoSimpleDescription("You are riding in Mr. Henrico's car. Prickly pine trees crowd both sides of the narrow, twisting road so all you can see is the road and a narrow strip of gray sky. Bits of fog cling to the low places and move through the trees like ghosts.\n\"Excited?\" Mr. Henrico asks. \"I would be. It's something of a dream of mine, I mean. To live on an island.\"");

            this.PerformDescribe().DoSimpleDescription("You are riding in Mr. Henrico's car. There is nothing to see besides the scenery.");

            Move(GetObject("Areas.Prologue.Henrico"), this);

            AddScenery("There are short fat pine trees and tall skinny ones. They are prickly and bare, with branches reaching out over the road here and there. They pass by too quickly to pick out a single tree for long. Nothing else seems to grow in whatever forest this is you're passing through.", "pines", "trees", "prickly", "pinetree", "pinetrees");

            AddScenery("The road is narrow, with no lines. Mr. Henrico clings to the right edge, but you haven't seen a vehicle pass you the other way yet, and you've been on this twisty road for at least fourty-five minutes. No streets cross it, there are no turn offs, no power lines or mailboxes. Just the road and the trees and the sky.", "road", "twisting", "narrow");

            AddScenery("The sky, what little you can see of it, is a uniform gray. It might rain, but it seems that the sky is more likely to stay the same depressing shade indefinitely.", "sky", "strip", "gray");

            AddScenery("The fog forms shapes as it drifts between the trees, shapes of everything and anything you can imagine. There it rears up with a wide snapping beak and spread wings, over there it is a man with a wide hat and a fishing pole.", "fog", "ghosts", "bits");

            AddScenery("noun, plural sceneries.\n1. the general appearance of a place; the aggregate of features that give character to a landscape.\n\nExcuse me, I meant, there are trees and things. Outside the car.", "scenery");

            AddScenery("Forest and sky and road stream by outside the car.", "outside");
            AddScenery("Don't be a smart ass.", "things");

            Check<MudObject, MudObject>("can go?").Do((actor, link) =>
                {
                    SendMessage(actor, "The car is moving. Rather fast, actually. You're going to stay put.");
                    return SharpRuleEngine.CheckResult.Disallow;
                });

        }
    }   
}