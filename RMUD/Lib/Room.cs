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

	public class Room : MudObject, Container
	{
        public RoomType RoomType = RoomType.Exterior;

		public List<MudObject> Contents = new List<MudObject>();
		public List<Link> Links = new List<Link>();
		public List<MudObject> Scenery = new List<MudObject>();

		public void OpenLink(Direction Direction, String Destination, MudObject Door = null)
		{
			Links.RemoveAll((l) => l.Direction == Direction);
            var link = new Link { Direction = Direction, Destination = Destination, Door = Door };
            link.Location = this;
            Links.Add(link);
		}

        #region Scenery 

        public Scenery AddScenery(String Description, params String[] Nouns)
		{
			var scenery = new Scenery();
			scenery.Long = Description;
			foreach (var noun in Nouns)
				scenery.Nouns.Add(noun.ToUpper());
            AddScenery(scenery);
            return scenery;
		}

        public void AddScenery(Scenery Scenery)
        {
            this.Scenery.Add(Scenery);
            Scenery.Location = this;
        }

        #endregion

        #region Implement IContainer

        public void Remove(MudObject Object)
        {
            if (Contents.Remove(Object))
                Object.Location = null;
            else if (Scenery.Remove(Object))
                Object.Location = null;
            else if (Links.RemoveAll(l => System.Object.ReferenceEquals(Object, l.Door)) > 0) { }
        }

        public void Add(MudObject MudObject, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default || (Locations & RelativeLocations.Contents) == RelativeLocations.Contents)
            {
                Contents.Add(MudObject);
                MudObject.Location = this;
            }
            else if ((Locations & RelativeLocations.Scenery) == RelativeLocations.Scenery)
            {
                Scenery.Add(MudObject);
                MudObject.Location = this;
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

        public RelativeLocations DefaultLocation { get { return RelativeLocations.Contents; } }

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
