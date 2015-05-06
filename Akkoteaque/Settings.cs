using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Akko
{
	public class settings : RMUD.Settings
	{
        public settings()
        {
            NewPlayerStartRoom = "Areas.Prologue.Car";
            PlayerBaseObject = "Player";
        }
	}
}
