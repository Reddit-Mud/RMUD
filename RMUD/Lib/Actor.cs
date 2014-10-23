using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : GenericContainer, TakeRules
	{
		public Client ConnectedClient;

		public override string Definite { get { return Short; } }
		public override string Indefinite { get { return Short; } }

        CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }

        public Actor()
            : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        { }

        public Dictionary<String, System.Collections.BitArray> ConversationKnowledge = new Dictionary<String, System.Collections.BitArray>();
        public NPC CurrentInterlocutor = null;
	}
}
