using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface PutRules
	{
		CheckRule CanPut(Actor Actor, Thing What, RelativeLocations Location);
		RuleHandlerFollowUp HandlePut(Actor Actor, Thing What, RelativeLocations Location);
	}
}
