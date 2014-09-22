using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : Thing, ITakeRules, IContainer
	{
		public Client ConnectedClient;
		public int Rank = 0;
		public List<Thing> Inventory = new List<Thing>();

		public override string Definite { get { return Short; } }
		public override string Indefinite { get { return Short; } }

		bool ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}

		void ITakeRules.HandleTake(Actor Actor) { }

		#region IContainer

		public void Remove(Thing Thing)
		{
			Inventory.Remove(Thing);
			Thing.Location = null;
		}

		public void Add(Thing Thing)
		{
			Inventory.Add(Thing);
			Thing.Location = this;
		}

		IEnumerator<Thing> IEnumerable<Thing>.GetEnumerator()
		{
			return Inventory.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (Inventory as System.Collections.IEnumerable).GetEnumerator();
		}

		#endregion
	}
}
