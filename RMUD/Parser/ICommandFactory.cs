using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class CommandFactory
	{
		public virtual void Create(CommandParser Parser)
		{
			throw new NotImplementedException();
		}
	}
}
