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

        public bool IsNamedObject { get { return Path != null; } }
        public bool IsAnonymousObject { get { return Path == null; } }
        public bool IsInstance { get { return IsNamedObject && Instance != null; } }
        public String GetFullName() { return Path + "@" + Instance; }

        public bool IsPersistent { get; set; }
        
		public virtual void Initialize() { }
        public virtual void HandleMarkedUpdate() { }
        public virtual void Heartbeat(UInt64 HeartbeatID) { }

        public virtual bool QueryQuestProperty(String Name) { return false; }
        public virtual void ResetQuest(Quest Quest) { }

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
        public MudObject Location { get; set; }

		public MudObject()
		{
			Nouns = new NounList();
            State = ObjectState.Alive;
            IsPersistent = false;
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
