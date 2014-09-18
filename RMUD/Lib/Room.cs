using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Room : MudObject, IContainer
	{
		public String Short;
		public DescriptiveText Long;

		public List<Thing> Contents = new List<Thing>();
		public List<Link> Links = new List<Link>();

		public void OpenLink(Direction Direction, String Destination)
		{
			Links.RemoveAll((l) => l.Direction == Direction);
			Links.Add(new Link { Direction = Direction, Destination = Destination });
		}

		public void Remove(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null) Contents.Remove(Thing);
		}

		public void Add(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null)
			{
				Contents.Add(Thing);
				Thing.Location = this;
			}
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
