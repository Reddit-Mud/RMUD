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
		public List<MudObject> Scenery = new List<MudObject>();

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

        public void Remove(MudObject Object)
        {
            if (Object is Thing && Contents.Remove(Object as Thing))
                (Object as Thing).Location = null;
            else if (Scenery.Remove(Object)) { }
            else if (Links.RemoveAll(l => System.Object.ReferenceEquals(Object, l.Door)) > 0) { }
        }

        public void Add(MudObject Thing, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default || (Locations & RelativeLocations.Contents) == RelativeLocations.Contents)
            {
                if (Thing is Thing)
                {
                    Contents.Add(Thing as Thing);
                    (Thing as Thing).Location = this;
                }
            }
            else if ((Locations & RelativeLocations.Scenery) == RelativeLocations.Scenery)
            {
                Scenery.Add(Thing);
            }
        }

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            if ((Locations & RelativeLocations.Contents) == RelativeLocations.Contents)
            {
                foreach (var mudObject in Contents)
                    if (Callback(mudObject, RelativeLocations.Contents) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }

            if ((Locations & RelativeLocations.Scenery) == RelativeLocations.Scenery)
            {
                foreach (var scenery in Scenery)
                    if (Callback(scenery, RelativeLocations.Scenery) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }

            if ((Locations & RelativeLocations.Links) == RelativeLocations.Links)
            {
                foreach (var link in Links)
                    if (link.Door != null && Callback(link.Door, RelativeLocations.Links) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }

            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if ((Locations & RelativeLocations.Contents) == RelativeLocations.Contents)
                return Contents.Contains(Object);
            else if ((Locations & RelativeLocations.Scenery) == RelativeLocations.Scenery)
                return Scenery.Contains(Object);
            else if ((Locations & RelativeLocations.Links) == RelativeLocations.Links)
                return Links.Count(l => System.Object.ReferenceEquals(Object, l.Door)) > 0;
            return false;
        }

        public RelativeLocations LocationsSupported
        {
            get
            {
                return RelativeLocations.Contents | RelativeLocations.Scenery | RelativeLocations.Links;
            }
        }

        public RelativeLocations LocationOf(MudObject Object)
        {
            if (Contents.Contains(Object)) return RelativeLocations.Contents;
            if (Scenery.Contains(Object)) return RelativeLocations.Scenery;
            return RelativeLocations.None;
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
                Mud.EnumerateObjects(this, EnumerateObjectsDepth.Deep, (t,l) =>
                {
                    if (t is EmitsLight)
                        if ((t as EmitsLight).EmitsLight)
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
