using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface TakeRules
	{
		CheckRule Check(Actor Actor);
		RuleHandlerFollowUp Handle(Actor Actor);
	}
}
