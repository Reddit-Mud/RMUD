using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Reflection;

namespace RMUD
{
	public partial class CommandFactory
	{
		public virtual void Create(CommandParser Parser)
		{
			throw new NotImplementedException();
        }

        public static CommandFactory CreateCommandFactory(Type Type)
        {
            return Activator.CreateInstance(Type) as CommandFactory;
        }
	}
}
