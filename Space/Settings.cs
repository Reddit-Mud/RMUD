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
                    actor.SetProperty("interlocutor", GetObject("Dan"));
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
