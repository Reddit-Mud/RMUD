using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class ContainerMutator : PersistentValueMutator
    {
        public override object PackValue(object Value)
        {
            var r = new Dictionary<String, List<String>>();
            foreach (var relativeLocation in (Value as Dictionary<RelativeLocations, List<MudObject>>))
                r.Upsert(relativeLocation.Key.ToString(), new List<String>(relativeLocation.Value.Where(o => o.IsNamedObject).Select(o => o.GetFullName())));
            return r;
        }

        public override object UnpackValue(object StoredValue)
        {
            var r = new Dictionary<RelativeLocations, List<MudObject>>();
            foreach (var relativeLocation in (StoredValue as Dictionary<String, List<String>>))
            {
                //The string is always of the form 'None, WhatWeActuallyWant'.
                var relativeLocationTokens = relativeLocation.Key.Split(new String[] { ", " }, StringSplitOptions.None);
                var relLoc = Enum.Parse(typeof(RelativeLocations), relativeLocationTokens[1]) as RelativeLocations?;
                r.Upsert(relLoc.Value, new List<MudObject>(relativeLocation.Value.Select(s => Mud.GetObject(s))));
            }
            return r;
        }
    }

	public class GenericContainer : MudObject, Container
	{
        [Persist(typeof(ContainerMutator))]
        public Dictionary<RelativeLocations, List<MudObject>> Lists { get; set; }

        public RelativeLocations Supported;
        public RelativeLocations Default;

        public GenericContainer(RelativeLocations Locations, RelativeLocations Default)
        {
            this.Supported = Locations;
            this.Default = Default;
            this.Lists = new Dictionary<RelativeLocations, List<MudObject>>();
        }

		#region IContainer

        public void Remove(MudObject Object)
        {
            foreach (var list in Lists)
            {
                if (list.Value.Remove(Object))
                    Object.Location = null;
            }
        }

		public void Add(MudObject Object, RelativeLocations Locations)
		{
            if (Locations == RelativeLocations.Default) Locations = Default;

            if ((Supported & Locations) == Locations)
            {
                if (!Lists.ContainsKey(Locations)) Lists.Add(Locations, new List<MudObject>());
                Lists[Locations].Add(Object);
            }
        }

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            if (Locations == RelativeLocations.Default) Locations = Default;

            foreach (var list in Lists)
            {
                if ((Locations & list.Key) == list.Key)
                    foreach (var MudObject in list.Value)
                        if (Callback(MudObject, list.Key) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default) Locations = Default;

            if (Lists.ContainsKey(Locations))
                return Lists[Locations].Contains(Object);
            return false;
        }

        public RelativeLocations LocationsSupported { get { return Supported; } }

        public RelativeLocations DefaultLocation { get { return Default; } }

        public RelativeLocations LocationOf(MudObject Object)
        {
            foreach (var list in Lists)
                if (list.Value.Contains(Object)) return list.Key;
            return RelativeLocations.None;
        }

		#endregion
	}
}
