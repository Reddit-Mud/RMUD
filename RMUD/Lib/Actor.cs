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
            GlobalRules.Check<MudObject, MudObject>("can-take").First.When((actor, thing) => thing is Actor).Do((actor, thing) =>
            {
                Mud.SendMessage(actor, "You can't take people.");
                return CheckResult.Disallow;
            });

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

	public class Actor : GenericContainer
	{
		public Client ConnectedClient;
        public List<StatusEffect> AppliedStatusEffects = new List<StatusEffect>();

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

        public void ApplyStatusEffect(StatusEffect Effect)
        {
            AppliedStatusEffects.Add(Effect);
            Effect.Apply(this);
        }

        public void RemoveStatusEffect(StatusEffect Effect)
        {
            AppliedStatusEffects.Remove(Effect);
            Effect.Remove(this);
        }

        public bool HasStatusEffect(Type StatusEffectType)
        {
            return AppliedStatusEffects.Count(se => StatusEffectType == se.GetType()) > 0;
        }

        public T GetStatusEffect<T>() where T: StatusEffect
        {
            foreach (var se in AppliedStatusEffects)
                if (se.GetType() == typeof(T)) return se as T;
            return null;
        }
       
	}
}
