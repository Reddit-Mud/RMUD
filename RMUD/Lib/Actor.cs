using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Actor : Thing, ITakeRules, IContainer
	{
		public Client ConnectedClient;
		public List<Thing> Inventory = new List<Thing>();

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
            if (!(Thing is Thing)) return;
			Inventory.Remove(Thing as Thing);
			(Thing as Thing).Location = null;
		}

		public void Add(MudObject Thing)
		{
            if (Thing is Thing)
            {
                Inventory.Add(Thing as Thing);
                (Thing as Thing).Location = this;
            }
		}

        public EnumerateObjectsControl EnumerateObjects(Func<MudObject, EnumerateObjectsControl> Callback)
        {
            foreach (var thing in Inventory)
                if (Callback(thing) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            return EnumerateObjectsControl.Continue;
        }

        public bool Contains(MudObject Object)
        {
            return Inventory.Contains(Object);
        }

		#endregion
	}
}
