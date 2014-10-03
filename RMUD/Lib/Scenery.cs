using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Scenery : MudObject, TakeRules
	{
		CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("That's a terrible idea.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }
	}
}
