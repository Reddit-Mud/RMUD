using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Room : MudObject
	{
		public String Short;
		public String Long;

		public List<Thing> Contents = new List<Thing>();
		public List<Link> Links = new List<Link>();

		public void OpenLink(Direction Direction, String Destination)
		{
			Links.RemoveAll((l) => l.Direction == Direction);
			Links.Add(new Link { Direction = Direction, Destination = Destination });
		}
	}
}
