using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Thing : MudObject
	{
		public String Short;
		public String Long;
		public String IndefiniteArticle = "a";
		public List<String> Nouns = new List<string>();

		public MudObject Location;

		public static void Move(Thing Thing, MudObject Destination)
		{
			if (Thing.Location != null)
			{
				var container = Thing.Location as IContainer;
				if (container != null)
					container.Remove(Thing);
				Thing.Location = null;
			}

			if (Destination != null)
			{
				var destinationContainer = Destination as IContainer;
				if (destinationContainer != null)
					destinationContainer.Add(Thing);
				Thing.Location = Destination;
			}
		}
	}
}
