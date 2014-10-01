using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface DropRules
	{
		CheckRule CanDrop(Actor Actor);
		RuleHandlerFollowUp HandleDrop(Actor Actor);
	}
}
