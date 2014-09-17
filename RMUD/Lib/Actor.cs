using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : Thing, Commands.ITakeRules, IContainer
	{
		public Client ConnectedClient;
		public int Rank = 0;
		public List<Thing> Inventory = new List<Thing>();

		bool Commands.ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}

		public void Remove(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null) Inventory.Remove(Thing);
		}

		public void Add(MudObject Object)
		{
			var Thing = Object as Thing;
			if (Thing != null)
			{
				Inventory.Add(Thing);
				Thing.Location = this;
			}
		}

		IEnumerator<MudObject> IEnumerable<MudObject>.GetEnumerator()
		{
			return Inventory.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (Inventory as System.Collections.IEnumerable).GetEnumerator();
		}
	}
}
