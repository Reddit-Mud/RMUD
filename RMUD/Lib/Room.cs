using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum RoomType
    {
        Exterior,
        Interior
    }

	public class Room : MudObject, IContainer, IDescribed
	{
		public String Short;
		public DescriptiveText Long { get; set; }
        public RoomType RoomType = RoomType.Exterior;

		public List<Thing> Contents = new List<Thing>();
		public List<Link> Links = new List<Link>();
		public List<Scenery> Scenery = new List<Scenery>();

		public void OpenLink(Direction Direction, String Destination, MudObject Door = null)
		{
			Links.RemoveAll((l) => l.Direction == Direction);
			Links.Add(new Link { Direction = Direction, Destination = Destination, Door = Door });
		}

        #region Scenery 

        public void AddScenery(String Description, params String[] Nouns)
		{
			var scenery = new Scenery();
			scenery.Long = Description;
			foreach (var noun in Nouns)
				scenery.Nouns.Add(noun.ToUpper());
			Scenery.Add(scenery);
		}

        public void AddScenery(Scenery Scenery)
        {
            this.Scenery.Add(Scenery);
        }

        #endregion

        #region Implement IContainer

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

        #endregion

        public bool IsLit { get; private set; }

        private void UpdateLighting()
        {
            IsLit = false;

            if (RoomType == RMUD.RoomType.Exterior)
            {
                IsLit = true;
                //Query day/night system to see if there is light
            }
            else
            {
                Mud.EnumerateObjects(this, EnumerateObjectsSettings.SingleRecurse | EnumerateObjectsSettings.VisibleOnly, t =>
                {
                    if (t is IEmitsLight)
                        if ((t as IEmitsLight).EmitsLight)
                        {
                            IsLit = true;
                            return EnumerateObjectsControl.Stop;
                        }
                    return EnumerateObjectsControl.Continue;
                });
            }
        }

        public override void HandleChanges()
        {
            UpdateLighting();
        }
    }
}
