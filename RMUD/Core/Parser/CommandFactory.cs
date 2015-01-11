using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Reflection;

namespace RMUD
{
	public partial class CommandFactory
	{
        internal RuleEngine GlobalRules;

		public virtual void Create(CommandParser Parser)
		{
			throw new NotImplementedException();
        }

        public static CommandFactory CreateCommandFactory(Type Type)
        {
            var r = Activator.CreateInstance(Type) as CommandFactory;
            r.GlobalRules = Core.GlobalRules;
            return r;
        }
	}
}
