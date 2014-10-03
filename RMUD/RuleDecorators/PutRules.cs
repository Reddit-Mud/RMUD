using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface PutRules
	{
		CheckRule Check(Actor Actor, MudObject What, RelativeLocations Location);
		RuleHandlerFollowUp Handle(Actor Actor, MudObject What, RelativeLocations Location);
	}
}
