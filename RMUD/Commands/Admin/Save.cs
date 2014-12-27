using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Save : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!SAVE")))
                .Manual("Saves all persistent objects to disc.")
                .ProceduralRule((match, actor) =>
                {
                    Mud.CommandTimeoutEnabled = false;

                    Mud.SendGlobalMessage("The database is being saved. There may be a brief delay.");
                    Mud.SendPendingMessages();

                    var saved = Mud.SaveActiveInstances();

                    Mud.SendGlobalMessage("The database has been saved.");
                    Mud.SendMessage(actor, String.Format("I saved {0} persistent objects.", saved));
                    return PerformResult.Continue;
                });
		}
	}

}
