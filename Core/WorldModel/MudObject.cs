using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class RegisterBaseProperties
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("short", typeof(String), "object");
            PropertyManifest.RegisterProperty("long", typeof(String), "");
            PropertyManifest.RegisterProperty("article", typeof(String), "a");
            PropertyManifest.RegisterProperty("nouns", typeof(NounList), new NounList());
        }
    }

	public partial class MudObject : SharpRuleEngine.RuleObject
    {
        public override SharpRuleEngine.RuleEngine GlobalRules { get { return Core.GlobalRules; } }

        /// Fundamental properties of every mud object: Don't mess with them.
        public ObjectState State = ObjectState.Unitialized; 
		public String Path { get; set; }
		public String Instance { get; set; }
        public bool IsNamedObject { get { return Path != null; } }
        public bool IsAnonymousObject { get { return Path == null; } }
        public bool IsInstance { get { return IsNamedObject && Instance != null; } }
        public String GetFullName() { return Path + "@" + Instance; }
        public bool IsPersistent { get; set; }
        public MudObject Location { get; set; }
        public override SharpRuleEngine.RuleObject LinkedRuleSource { get { return Location; } }

        public virtual void Initialize() { }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Path)) return this.GetType().Name;
            else return Path;
        }

        #region Properties

        // Every MudObject has a set of generic properties. Modules use these properties to store values on MudObjects.
                
        public Dictionary<String, Object> Properties = new Dictionary<string, Object>();
                

        public void SetProperty(String Name, Object Value)
        {
            if (PropertyManifest.CheckPropertyType(Name, Value))
                Properties.Upsert(Name, Value);
            else
                throw new InvalidOperationException("Setting property with object of wrong type.");
        }

        public T GetProperty<T>(String Name)
        {
            if (Properties.ContainsKey(Name))
                return (T)Properties[Name];
            else
                throw new InvalidOperationException("Mud Object does not have a property named " + Name);
        }

        public T GetPropertyOrDefault<T>(String Name)
        {
            if (Properties.ContainsKey(Name))
                return (T)Properties[Name];
            else
            {
                var info = PropertyManifest.GetPropertyInformation(Name);
                if (info == null)
                    throw new InvalidOperationException("Property " + Name + " does not exist.");
                return (T)info.DefaultValue;
            }
        }
        
        public bool HasProperty(String Name)
        {
            return Properties.ContainsKey(Name);
        }

        #endregion

		public MudObject()
		{
		    State = ObjectState.Alive;
            IsPersistent = false;
		}

        public MudObject(String Short, String Long)
        {
            SetProperty("short", Short);
            SetProperty("long", Long);
            GetProperty<NounList>("nouns").Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                SetProperty("article", "an");

            State = ObjectState.Alive;
            IsPersistent = false;

        }

        public void SimpleName(String Short, params String[] Synonyms)
        {
            SetProperty("short", Short);
            GetProperty<NounList>("nouns").Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            GetProperty<NounList>("nouns").Add(Synonyms);
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

            if (DestroyChildren)
                foreach (var child in EnumerateObjects())
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
