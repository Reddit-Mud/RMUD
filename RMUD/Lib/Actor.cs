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

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, String, String>("actor-name", "[Viewer, Actor -> String] : Find the name that should be displayed for an actor.");
            
            GlobalRules.Value<MudObject, MudObject, String, String>("actor-name")
                .When((viewer, actor, article) => !(viewer is Actor) || !(actor is Actor))
                .Do((viewer, actor, article) => article + " " + actor.Short)
                .Name("Name of non-actor.");
            
            GlobalRules.Value<MudObject, MudObject, String, String>("actor-name")
                .When((viewer, actor, article) =>
                Introduction.ActorKnowsActor(viewer as Actor, actor as Actor))
                .Do((viewer, actor, article) => actor.Short)
                .Name("Name of introduced actor.");

            GlobalRules.Value<MudObject, MudObject, String, String>("actor-name")
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

        private string PrepareName(Actor RequestedBy, String Article)
        {
            return GlobalRules.ConsiderValueRule<String>("actor-name", this, RequestedBy, this, Article);
        }

        public override string Definite(Actor RequestedBy)
        {
            return PrepareName(RequestedBy, "the");
        }

        public override string Indefinite(Actor RequestedBy)
        {
            return PrepareName(RequestedBy, Article);
        }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        {
            Gender = RMUD.Gender.Male;
            Nouns.Add("MAN", (a) => a.Gender == RMUD.Gender.Male);
            Nouns.Add("WOMAN", (a) => a.Gender == RMUD.Gender.Female);
        }

        public override void Heartbeat(ulong HeartbeatID)
        {
            foreach (var effect in new List<StatusEffect>(AppliedStatusEffects))
                effect.Heartbeat(HeartbeatID, this);
            base.Heartbeat(HeartbeatID);
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
