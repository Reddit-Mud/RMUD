using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class GenericContainer : Thing, IContainer
	{
        public Dictionary<RelativeLocations, List<MudObject>> Lists = new Dictionary<RelativeLocations, List<MudObject>>();
        public RelativeLocations Supported;

        public GenericContainer(RelativeLocations Locations)
        {
            this.Supported = Locations;
        }

		#region IContainer

        public void Remove(MudObject Object)
        {
            foreach (var list in Lists)
            {
                if (list.Value.Remove(Object) && Object is Thing)
                    (Object as Thing).Location = null;
            }
        }

		public void Add(MudObject Object, RelativeLocations Locations)
		{
            if ((Supported & Locations) == Locations)
            {
                if (!Lists.ContainsKey(Locations)) Lists.Add(Locations, new List<MudObject>());
                Lists[Locations].Add(Object);
            }
        }

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            foreach (var list in Lists)
            {
                if ((Locations & list.Key) == list.Key)
                    foreach (var thing in list.Value)
                        if (Callback(thing, list.Key) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if (Lists.ContainsKey(Locations))
                return Lists[Locations].Contains(Object);
            return false;
        }

        public RelativeLocations LocationsSupported { get { return Supported; } }

        public RelativeLocations LocationOf(MudObject Object)
        {
            foreach (var list in Lists)
                if (list.Value.Contains(Object)) return list.Key;
            return RelativeLocations.None;
        }

		#endregion
	}
}
