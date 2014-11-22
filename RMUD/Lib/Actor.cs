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

       
	}
}
