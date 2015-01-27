using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Space
{
	public class settings : RMUD.Settings
	{
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.Perform<Player>("player joined")
                .Do((actor) =>
                {
                    SendMessage(actor, "Sal? Sal? Can you hear me?");
                    (actor as Player).CurrentInterlocutor = GetObject("Dan") as NPC;
                    Core.EnqueuActorCommand(actor, "topics");
                    return PerformResult.Stop;
                });
        }

        public settings()
        {
            NewPlayerStartRoom = "Start";
            PlayerBaseObject = "Player";
        }
	}
}
