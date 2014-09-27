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

		bool ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}

        RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor) { return RuleHandlerFollowUp.Continue; }
	}
}
