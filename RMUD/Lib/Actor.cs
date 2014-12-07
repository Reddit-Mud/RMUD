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
            var introduced = Introduction.ActorKnowsActor(RequestedBy, this);
            var result = introduced ? Short : DescriptiveName;

            foreach (var statusEffect in AppliedStatusEffects)
            {
                var modifiedName = statusEffect.OverrideName(this, result);
                if (modifiedName.Item1 == true) introduced = false; //True means discard introduction data.
                result = modifiedName.Item2;
            }

            if (introduced) return result;
            else return Article + " " + result;
        }

        public override string Definite(Actor RequestedBy)
        {
            return PrepareName(RequestedBy, "the");
        }

        public override string Indefinite(Actor RequestedBy)
        {
            return PrepareName(RequestedBy, Article);
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
