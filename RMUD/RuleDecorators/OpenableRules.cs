using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	/// <summary>
	/// Decorates an object that can be open or closed, and interactive with the open and close commands.
	/// </summary>
	public interface OpenableRules
	{
		bool Open { get; }
		CheckRule CheckOpen(Actor Actor);
		CheckRule CheckClose(Actor Actor);
		RuleHandlerFollowUp HandleOpen(Actor Actor);
		RuleHandlerFollowUp HandleClose(Actor Actor);
	}
}
