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

		public bool Is(String other) 
		{
			return Path == other;
		}

		public DTO GetDTO()
		{
			//Use the name 'Path@Instance' to fetch data from the dynamic database.
			// If Instance is null or empty, the resulting name is 'Path@'. This is the
			// name non-instanced objects can use to store data in the dynamic
			// database.
            return Mud.LoadDTO(Path + "@" + Instance);
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
		public MudObject Location;

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
            if (!(Object is MudObject)) return; //Can't move it if it isn't a MudObject..

            var MudObject = Object as MudObject;

			if (MudObject.Location != null)
			{
				var container = MudObject.Location as Container;
				if (container != null)
					container.Remove(MudObject);
				MudObject.Location = null;
			}

			if (Destination != null)
			{
				var destinationContainer = Destination as Container;
				if (destinationContainer != null)
					destinationContainer.Add(MudObject, Location);
				MudObject.Location = Destination;
			}
		}

    }
}
