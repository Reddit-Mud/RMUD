using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class BasicDoor : Thing, IOpenableRules, ITakeRules
	{
		public BasicDoor()
		{
			this.Nouns.Add("DOOR");
		}

		#region IOpenable

		public bool Open { get; set; }

		bool IOpenableRules.CanOpen(Actor Actor)
		{
			return !Open;
		}

		bool IOpenableRules.CanClose(Actor Actor)
		{
			return Open;
		}

		void IOpenableRules.HandleOpen(Actor Actor)
		{
			Open = true;
		}

		void IOpenableRules.HandleClose(Actor Actor)
		{
			Open = false;
		}

		#endregion

		bool ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}
	}
}
