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

	public class Actor : GenericContainer, TakeRules
	{
		public Client ConnectedClient;
        public List<StatusEffect> AppliedStatusEffects = new List<StatusEffect>();

        [Persist(typeof(EnumSerializer<Gender>))]
        public Gender Gender { get; set; }

        public String DescriptiveName { get; set; }

        public override string Definite(Actor RequestedBy)
        {
            if (Introduction.ActorKnowsActor(RequestedBy, this)) return Short;
            return "the " + DescriptiveName;
        }

		public override string Indefinite(Actor RequestedBy)
        {
            if (Introduction.ActorKnowsActor(RequestedBy, this)) return Short;
            return "a " + DescriptiveName;
        }

        CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        {
            Gender = RMUD.Gender.Male;
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
