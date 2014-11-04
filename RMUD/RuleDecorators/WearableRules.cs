using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface WearableRules
	{
		CheckRule CheckWear(Actor Actor);
		CheckRule CheckRemove(Actor Actor);
		RuleHandlerFollowUp HandleWear(Actor Actor);
		RuleHandlerFollowUp HandleRemove(Actor Actor);
	}
}
