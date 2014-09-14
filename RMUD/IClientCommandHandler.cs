using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IClientCommandHandler
	{
		void HandleCommand(Client Client, String Command);
	}
}
