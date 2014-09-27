using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Scenery : MudObject, IDescribed, IMatchable, ITakeRules
	{
		public NounList Nouns { get; set; }
		public DescriptiveText Long { get; set; }

		public Scenery()
		{
			Nouns = new NounList();
		}

		CheckRule ITakeRules.CanTake(Actor Actor)
		{
			return CheckRule.Disallow("That's a terrible idea.");
		}

        RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor) { return RuleHandlerFollowUp.Continue; }
	}
}
