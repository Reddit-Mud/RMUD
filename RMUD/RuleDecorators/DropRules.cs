using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface DropRules
	{
		CheckRule Check(Actor Actor);
		RuleHandlerFollowUp Handle(Actor Actor);
	}

    //Invoked for every object that witnesses an item being dropped.
    public interface WitnessDropRules
    {
        void Handle(Actor Actor, MudObject Item);
    }
}
