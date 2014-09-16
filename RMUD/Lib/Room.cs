using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Room : MudObject, IContainer
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

		void IContainer.Remove(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null) Contents.Remove(Thing);
		}

		void IContainer.Add(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null) Contents.Add(Thing);
		}

		IEnumerator<MudObject> IEnumerable<MudObject>.GetEnumerator()
		{
			return Contents.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (Contents as System.Collections.IEnumerable).GetEnumerator();
		}
	}
}
