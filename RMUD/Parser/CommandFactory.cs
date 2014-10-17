using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Reflection;

namespace RMUD
{
	public class CommandFactory
	{
		public virtual void Create(CommandParser Parser)
		{
			throw new NotImplementedException();
		}

        private static Dictionary<String, System.Type> _AllCommands = null;

        public static CommandFactory GetCommand(string CommandName)
        {
            if (_AllCommands == null)
                InitializeCommands();

            CommandFactory cmd = null;
            System.Type cmdType = null;
            if (_AllCommands.TryGetValue(CommandName, out cmdType))
                cmd = Activator.CreateInstance(cmdType) as CommandFactory;
            return cmd;
        }

        public static Dictionary<String, System.Type> AllCommands
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
            _AllCommands = new Dictionary<String, System.Type>();
            //Iterate over all types, find ICommandFactories, Store instance
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(CommandFactory)))
                {
                    _AllCommands.Add(type.Name, type);
                }
            }
        }
	}
}
