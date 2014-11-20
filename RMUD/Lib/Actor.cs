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

		public override string Definite(MudObject RequestedBy) { return Short; }
		public override string Indefinite(MudObject RequestedBy) { return Short; }

        CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        { }

       
	}
}
