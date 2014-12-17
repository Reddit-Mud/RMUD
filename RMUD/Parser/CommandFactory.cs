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

        private static List<System.Type> _AllCommands = null;

        public static CommandFactory GetCommand(Type Type)
        {
            return Activator.CreateInstance(Type) as CommandFactory;
        }

        public static List<System.Type> AllCommands
        {
            get
            {
                if (_AllCommands == null)
                    InitializeCommands();
                return _AllCommands;
            }

            private set { }
        }

        private static void InitializeCommands()
        {
            _AllCommands = new List<System.Type>();
            //Iterate over all types, find ICommandFactories, Store instance
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(CommandFactory)))
                {
                    _AllCommands.Add(type);
                }
            }
        }
	}
}
