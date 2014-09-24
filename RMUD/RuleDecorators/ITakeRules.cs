using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RuleHandlerFollowUp
    {
        Stop,
        Continue,
    }

	public interface ITakeRules
	{
		bool CanTake(Actor Actor);
		RuleHandlerFollowUp HandleTake(Actor Actor);
	}
}
