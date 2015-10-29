using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public partial class MudObject : SharpRuleEngine.RuleObject
    {
        public override SharpRuleEngine.RuleEngine GlobalRules { get { return Core.GlobalRules; } }

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

        public String Short = "object";
        public String Long = "";
        public String Article = "a";
		public NounList Nouns { get; set; }

        public MudObject Location { get; set; }
        public override SharpRuleEngine.RuleObject LinkedRuleSource { get { return Location; } }

        #region Properties

        // Every MudObject has a set of generic properties. Modules use these properties to store values on MudObjects.
        
        public Dictionary<String, Object> Properties = null;

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

        public T GetPropertyOrDefault<T>(String Name, T Default)
        {
            if (Properties == null) return Default;
            if (Properties.ContainsKey(Name) && Properties[Name] is T) return (T)Properties[Name];
            return Default;
        }
        
        public bool GetBooleanProperty(String Name)
        {
            return GetPropertyOrDefault<bool>(Name, false);
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

        public void RemoveProperty(String Name)
        {
            if (Properties != null) Properties.Remove(Name);
        }
        
        #endregion

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
            Nouns.Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                Article = "an";

            State = ObjectState.Alive;
            IsPersistent = false;

        }

        public void SimpleName(String Short, params String[] Synonyms)
        {
            this.Short = Short;
            Nouns.Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            Nouns.Add(Synonyms);
        }

        /// <summary>
        /// Destroy this object. Optionally, destroy it's children. If destroying children, destroy the
        /// children's children, etc. The most important aspect of this function is that when destroyed,
        /// persistent objects are forgotten. Destroying non-persistent objects is not necessary.
        /// </summary>
        /// <param name="DestroyChildren"></param>
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

        public static MudObject InitializeObject(MudObject Object)
        {
            Object.Initialize();
            Object.State = ObjectState.Alive;
            Core.GlobalRules.ConsiderPerformRule("update", Object);
            return Object;
        }

        public static T GetObject<T>(String Path) where T: MudObject
        {
            return GetObject(Path) as T;
        }
    }
}
