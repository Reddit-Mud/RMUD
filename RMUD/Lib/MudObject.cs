using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum ObjectState
    {
        Unitialized,
        Alive,
        Destroyed,
    }

	public class MudObject
    {
        public ObjectState State = ObjectState.Unitialized; 
		public String Path { get; internal set; }
		public String Instance { get; internal set; }
       
        public DTO PersistenceObject { get; internal set; }
        public bool IsPersistent { get { return PersistenceObject != null; } }
        
		public bool Is(String other) 
		{
			return Path == other;
		}
        
		public virtual void Initialize() { }
        public virtual void HandleMarkedUpdate() { }
        public virtual void Heartbeat(UInt64 HeartbeatID) { }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Path)) return this.GetType().Name;
            else return Path;
        }

        public String Short;
		public DescriptiveText Long { get; set; }
        public String Article = "a";
		public virtual String Indefinite { get { return Article + " " + Short; } }
		public virtual String Definite { get { return "the " + Short; } }
		public NounList Nouns { get; set; }

        [Persist(typeof(LocationMutator))]
        public MudObject Location { get; set; }

        private class LocationMutator : PersistentValueMutator
        {
            public override object MutateValue(object Value)
            {
                var mudObject = Value as MudObject;
                if (mudObject == null) return null;
                return mudObject.Path; //This is incomplete, but this is likely to be replaced by
                //persisting the parent container anyway. 
            }
        }
        
		public MudObject()
		{
			Nouns = new NounList();
            State = ObjectState.Alive;
		}

        public MudObject(String Short, String Long)
        {
            Nouns = new NounList();
            this.Short = Short;
            this.Long = Long;
            Nouns.AddRange(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                Article = "an";

            State = ObjectState.Alive;
        }

        public void Destroy(bool DestroyChildren)
        {
            State = ObjectState.Destroyed;
            Mud.ForgetInstance(this);

            if (DestroyChildren && this is Container)
                (this as Container).EnumerateObjects(RelativeLocations.EveryMudObject, (child, loc) =>
                    {
                        if (child.State != ObjectState.Destroyed)
                            child.Destroy(true);
                        return EnumerateObjectsControl.Continue;
                    });
        }

		public static void Move(MudObject Object, MudObject Destination, RelativeLocations Location = RelativeLocations.Default)
		{
            if (Object.Location != null)
			{
                var container = Object.Location as Container;
				if (container != null)
                    container.Remove(Object);
                Object.Location = null;
			}

			if (Destination != null)
			{
				var destinationContainer = Destination as Container;
				if (destinationContainer != null)
                    destinationContainer.Add(Object, Location);
                Object.Location = Destination;
			}
		}
    }
}
