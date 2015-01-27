using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public partial class MudObject
    {
        public ObjectState State = ObjectState.Unitialized; 
		public String Path { get; set; }
		public String Instance { get; set; }

        public bool IsNamedObject { get { return Path != null; } }
        public bool IsAnonymousObject { get { return Path == null; } }
        public bool IsInstance { get { return IsNamedObject && Instance != null; } }
        public String GetFullName() { return Path + "@" + Instance; }

        public bool IsPersistent { get; set; }
        
		public virtual void Initialize() { }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Path)) return this.GetType().Name;
            else return Path;
        }

        public String Short;
        public String Long;
        public String Article = "a";
		public NounList Nouns { get; set; }
        public MudObject Location { get; set; }
        public RuleSet Rules { get; set; }
        
        public String Indefinite(MudObject RequestedBy) 
        {
            return GlobalRules.ConsiderValueRule<String>("printed name", RequestedBy, this, Article);
        }

        public String Definite(MudObject RequestedBy)
        {
            return GlobalRules.ConsiderValueRule<String>("printed name", RequestedBy, this, "the");
        }

        #region Properties
        
        private Dictionary<String, Object> Properties = null;

        public void SetProperty(String Name, Object Value)
        {
            if (Properties == null) Properties = new Dictionary<string, object>();
            Properties.Upsert(Name, Value);
        }

        public T GetProperty<T>(String Name)
        {
            if (Properties == null) return default(T);
            return (T)Properties.ValueOrDefault(Name);
        }

        public bool HasProperty(String Name)
        {
            if (Properties == null) return false;
            return Properties.ContainsKey(Name);
        }

        public bool HasProperty<T>(String Name)
        {
            if (Properties == null) return false;
            return Properties.ContainsKey(Name) && Properties[Name] is T;
        }
        
        #endregion

		public MudObject()
		{
			Nouns = new NounList();
            State = ObjectState.Alive;
            IsPersistent = false;
            Rules = null;
		}

        public MudObject(String Short, String Long)
        {
            Nouns = new NounList();
            this.Short = Short;
            this.Long = Long;
            Nouns.Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                Article = "an";

            State = ObjectState.Alive;
            IsPersistent = false;

            Rules = null;
        }

        public void SimpleName(String Short, params String[] Synonyms)
        {
            this.Short = Short;
            Nouns.Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            Nouns.Add(Synonyms);
        }

        public void Destroy(bool DestroyChildren)
        {
            State = ObjectState.Destroyed;
            MudObject.ForgetInstance(this);

            if (DestroyChildren && this is Container)
                foreach (var child in (this as Container).EnumerateObjects())
                    if (child.State != ObjectState.Destroyed)
                        child.Destroy(true);
        }

        public static MudObject GetObject(String Path)
        {
            return Core.Database.GetObject(Path);
        }

        public static T GetObject<T>(String Path) where T: MudObject
        {
            return GetObject(Path) as T;
        }
    }
}
