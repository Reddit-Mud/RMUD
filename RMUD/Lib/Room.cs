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

        public void Remove(MudObject Thing)
		{
            if (Thing is Thing)
            {
                Contents.Remove(Thing as Thing);
                (Thing as Thing).Location = null;
            }
		}

		public void Add(MudObject Thing)
		{
            if (Thing is Thing)
            {
                Contents.Add(Thing as Thing);
                (Thing as Thing).Location = this;
            }
		}

        public EnumerateObjectsControl EnumerateObjects(Func<MudObject, EnumerateObjectsControl> Callback)
        {
            foreach (var mudObject in Contents)
                if (Callback(mudObject) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            foreach (var scenery in Scenery)
                if (Callback(scenery) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            foreach (var link in Links)
                if (link.Door != null && Callback(link.Door) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object)
        {
            return Contents.Contains(Object);
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
                Mud.EnumerateObjects(this, EnumerateObjectsDepth.Deep, t =>
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

        public override void HandleMarkedUpdate()
        {
            UpdateLighting();
        }
    }
}
