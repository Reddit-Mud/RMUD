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

            GlobalRules.Value<MudObject, MudObject, String, String>("printed-name")
                .When((viewer, thing, article) => viewer is Actor && thing is Actor && Introduction.ActorKnowsActor(viewer as Actor, thing as Actor))
                .Do((viewer, actor, article) => actor.Short)
                .Name("Name of introduced actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed-name")
                .When((viewer, thing, article) => thing is Actor)
                .Do((viewer, actor, article) => article + " " + (actor as Actor).DescriptiveName)
                .Name("Default name for unintroduced actor.");
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
