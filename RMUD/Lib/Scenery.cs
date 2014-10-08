using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Scenery : MudObject, TakeRules
	{
        public Scenery() { }

        public Scenery(String Short, String Long) : base(Short, Long) { }

		CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("That's a terrible idea.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }
	}
}
