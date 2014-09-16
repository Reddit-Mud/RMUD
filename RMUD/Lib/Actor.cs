using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : Thing
	{
		public Client ConnectedClient;
		public int Rank = 0;
	}
}
