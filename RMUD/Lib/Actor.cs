using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum Gender
    {
        Male,
        Female
    }

    public class ActorRules : DeclaresRules
    {
        public void InitializeGlobalRules()
        {
            GlobalRules.Check<MudObject, MudObject>("can take?")
                .First
                .When((actor, thing) => thing is Actor)
                .Do((actor, thing) =>
                {
                    Mud.SendMessage(actor, "You can't take people.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take people rule.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => viewer is Actor && thing is Actor && Introduction.ActorKnowsActor(viewer as Actor, thing as Actor))
                .Do((viewer, actor, article) => actor.Short)
                .Name("Name of introduced actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing is Actor && (thing as Actor).Gender == Gender.Male)
                .Do((viewer, actor, article) => article + " man")
                .Name("Default name for unintroduced male actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing is Actor && (thing as Actor).Gender == Gender.Female)
                .Do((viewer, actor, article) => article + " woman")
                .Name("Default name for unintroduced female actor.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .First
                .When((viewer, item) => item is Actor)
                .Do((viewer, item) =>
                {
                    var actor = item as Actor;
                    if (viewer is Actor && Introduction.ActorKnowsActor(viewer as Actor, actor))
                        Mud.SendMessage(viewer, "^<the0>, a " + (actor.Gender == Gender.Male ? "man." : "woman."), actor);

                    var wornItems = new List<Clothing>(actor.EnumerateObjects<Clothing>(RelativeLocations.Worn));
                    if (wornItems.Count == 0)
                        Mud.SendMessage(viewer, "^<the0> is naked.", actor);
                    else
                        Mud.SendMessage(viewer, "^<the0> is wearing " + String.Join(", ", wornItems.Select(c => c.Indefinite(viewer))) + ".", actor);

                    var heldItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Held));
                    if (heldItems.Count == 0)
                        Mud.SendMessage(viewer, "^<the0> is empty handed.", actor);
                    else
                        Mud.SendMessage(viewer, "^<the0> is holding " + String.Join(", ", heldItems.Select(i => i.Indefinite(viewer))) + ".", actor);

                    return PerformResult.Continue;
                })
                .Name("List worn and held items when describing an actor rule.");
        }
    }

    public class Actor : Container
    {
        public Client ConnectedClient;

        [Persist(typeof(EnumSerializer<Gender>))]
        public Gender Gender { get; set; }

        public String DescriptiveName
        {
            get
            {
                if (Gender == Gender.Male)
                    return "man";
                else
                    return "woman";
            }
        }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        {
            Gender = RMUD.Gender.Male;
            Nouns.Add("MAN", (a) => a.Gender == RMUD.Gender.Male);
            Nouns.Add("WOMAN", (a) => a.Gender == RMUD.Gender.Female);
        }

    }
}
