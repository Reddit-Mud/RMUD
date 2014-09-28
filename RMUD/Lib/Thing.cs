using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Thing : MudObject, IDescribed, IMatchable
	{
		public String Short;
		public DescriptiveText Long { get; set; }
        public String Article = "a";
		public virtual String Indefinite { get { return Article + " " + Short; } }
		public virtual String Definite { get { return "the " + Short; } }
		public NounList Nouns { get; set; }
		public MudObject Location;

		public Thing()
		{
			Nouns = new NounList();
		}

		public static void Move(MudObject Object, MudObject Destination)
		{
            if (!(Object is Thing)) return; //Can't move it if it isn't a thing..

            var Thing = Object as Thing;

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
