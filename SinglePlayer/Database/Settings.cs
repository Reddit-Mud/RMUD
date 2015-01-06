using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD.Modules.Chat;

namespace SinglePlayer.Database
{
	public class settings : RMUD.Settings
	{
        public settings()
        {
            NewPlayerStartRoom = "Foyer";
        }
	}
}
