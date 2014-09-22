using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace RMUD
{
	public class Room : MudObject, IContainer, IDescribed
	{
		public String Short;
		public DescriptiveText Long { get; set; }

		public List<Thing> Contents = new List<Thing>();
		public List<Link> Links = new List<Link>();
		public List<Scenery> Scenery = new List<Scenery>();

		public void OpenLink(Direction Direction, String Destination, MudObject Door = null)
		{
			Links.RemoveAll((l) => l.Direction == Direction);
			Links.Add(new Link { Direction = Direction, Destination = Destination, Door = Door });
		}

		public void AddScenery(String Description, params String[] Nouns)
		{
			var scenery = new Scenery();
			scenery.Long = Description;
			foreach (var noun in Nouns)
				scenery.Nouns.Add(noun.ToUpper());
			Scenery.Add(scenery);
		}

		public void Remove(Thing Thing)
		{
			Contents.Remove(Thing);
			Thing.Location = null;
		}

		public void Add(Thing Thing)
		{
			Contents.Add(Thing);
			Thing.Location = this;
		}

		IEnumerator<Thing> IEnumerable<Thing>.GetEnumerator()
		{
			return Contents.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (Contents as System.Collections.IEnumerable).GetEnumerator();
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
