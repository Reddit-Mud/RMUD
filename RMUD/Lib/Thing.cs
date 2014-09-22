using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace RMUD
{
	public class Thing : MudObject, IDescribed, IMatchable
	{
		public String Short;
		public DescriptiveText Long { get; set; }
		public virtual String Indefinite { get { return "a " + Short; } }
		public virtual String Definite { get { return "the " + Short; } }
		public List<String> Nouns { get; set; }
		public MudObject Location;

		public Thing()
		{
			Nouns = new List<string>();
		}

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

	    public override void Save()
	    {
	        base.Save();
            using (IDocumentSession session = Mud.PersistentStore.OpenSession())
            {
                //Thing existingStore = session.Load<Thing>(this.Id);
                session.Store(this);
                session.SaveChanges();
            }
            
	    }
	}
}
