using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RMUD
{
    internal class ClientCommand : Action
    {
        internal Client Executor;
        internal String RawCommand;

        internal ClientCommand(Client Executor, String RawCommand)
        {
            this.Executor = Executor;
            this.RawCommand = RawCommand;
        }

        public override void Execute(MudCore core)
        {
			try
			{
				var obj = core.Database.LoadObject(RawCommand);
				core.SendMessage(Executor, obj.Path, true);
			}
			catch (Exception e)
			{
				core.SendMessage(Executor, e.Message, true);
			}
        }
    }
}