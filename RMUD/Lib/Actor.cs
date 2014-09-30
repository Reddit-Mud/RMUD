using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : Thing, ITakeRules, IContainer
	{
		public Client ConnectedClient;
		public List<Thing> Held = new List<Thing>();

		public override string Definite { get { return Short; } }
		public override string Indefinite { get { return Short; } }

        CheckRule ITakeRules.CanTake(Actor Actor)
		{
			return CheckRule.Disallow("You can't take people.");
		}

        RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor) { return RuleHandlerFollowUp.Continue; }

		#region IContainer

		public void Remove(MudObject Thing)
		{
            if (Thing is Thing)
            {
                
                    if (Held.Remove(Thing as Thing))
                        (Thing as Thing).Location = null;
                
            }
		}

		public void Add(MudObject Thing, RelativeLocations Locations)
		{
            if (Thing is Thing)
            {
                if (Locations == RelativeLocations.Default || (Locations & RelativeLocations.Held) == RelativeLocations.Held)
                {
                    Held.Add(Thing as Thing);
                    (Thing as Thing).Location = this;
                }
            }
		}

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            if ((Locations & RelativeLocations.Held) == RelativeLocations.Held)
                foreach (var thing in Held)
                    if (Callback(thing, RelativeLocations.Held) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if ((Locations & RelativeLocations.Held) == RelativeLocations.Held)
                return Held.Contains(Object);
            return false;
        }

        public RelativeLocations LocationsSupported { get { return RelativeLocations.Held | RelativeLocations.Worn; } }

        public RelativeLocations LocationOf(MudObject Object)
        {
            if (Held.Contains(Object)) return RelativeLocations.Held;
            return RelativeLocations.None;
        }

		#endregion
	}
}
