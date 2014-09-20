using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface ITakeRules
	{
		bool CanTake(Actor Actor);
	}
}
