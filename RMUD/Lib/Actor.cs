using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : MudObject, TakeRules, Container
	{
		public Client ConnectedClient;
		public List<MudObject> Held = new List<MudObject>();
        public List<MudObject> Worn = new List<MudObject>();

		public override string Definite { get { return Short; } }
		public override string Indefinite { get { return Short; } }

        CheckRule TakeRules.Check(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp TakeRules.Handle(Actor Actor) { return RuleHandlerFollowUp.Continue; }

		#region IContainer

        public void Remove(MudObject MudObject)
        {
            if (Held.Remove(MudObject))
                MudObject.Location = null;
            else if (Worn.Remove(MudObject))
                MudObject.Location = null;
        }

        public void Add(MudObject MudObject, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default || (Locations & RelativeLocations.Held) == RelativeLocations.Held)
            {
                Held.Add(MudObject);
                MudObject.Location = this;
            }
            else if ((Locations & RelativeLocations.Worn) == RelativeLocations.Worn)
            {
                Worn.Add(MudObject);
                MudObject.Location = this;
            }
        }

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            if ((Locations & RelativeLocations.Held) == RelativeLocations.Held)
                foreach (var MudObject in Held)
                    if (Callback(MudObject, RelativeLocations.Held) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            if ((Locations & RelativeLocations.Worn) == RelativeLocations.Worn)
                foreach (var MudObject in Worn)
                    if (Callback(MudObject, RelativeLocations.Worn) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if ((Locations & RelativeLocations.Held) == RelativeLocations.Held)
                return Held.Contains(Object);
            else if ((Locations & RelativeLocations.Worn) == RelativeLocations.Worn)
                return Worn.Contains(Object);
            return false;
        }

        public RelativeLocations LocationsSupported { get { return RelativeLocations.Held | RelativeLocations.Worn; } }

        public RelativeLocations DefaultLocation { get { return RelativeLocations.Held; } }

        public RelativeLocations LocationOf(MudObject Object)
        {
            if (Held.Contains(Object)) return RelativeLocations.Held;
            if (Worn.Contains(Object)) return RelativeLocations.Worn;
            return RelativeLocations.None;
        }

		#endregion
	}
}
