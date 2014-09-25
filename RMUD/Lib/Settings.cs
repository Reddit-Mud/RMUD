using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Settings : MudObject
	{
		public String Banner;
		public String MessageOfTheDay;
        public String NewPlayerStartRoom;

        public int AllowedCommandRate = 100; //How many milliseconds to allow between commands - default is to not limit very much.

        public String ProscriptionList = "proscriptions.txt";
	}
}
