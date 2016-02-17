using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
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
                
        public Dictionary<String, MudObjectProperty> Properties = new Dictionary<string, MudObjectProperty>();

        public void AddProperty(String Name, System.Type Type)
        {
            if (Properties.ContainsKey(Name))
            {
                Core.LogWarning(String.Format("Add property erased previous value: {0}.", Name));
                if (Properties[Name].PropertyType != Type)
                {
                    Core.LogWarning(String.Format("Add property changed type: {0} changed to type {1}.", Name, Type.Name));
                }
            }

            Properties.Upsert(Name, new MudObjectProperty(Type));
        }

        public void SetProperty(String Name, Object Value)
        {
            MudObjectProperty property = null;
            if (!Properties.TryGetValue(Name, out property))
            {
                if (Value == null)
                {
                    Core.LogWarning(String.Format("Implicit property creation with NULL: {0}.", Name));
                    UpsertProperty(Name, typeof(Object), null);
                }
                else
                {
                    Core.LogWarning(String.Format("Implicit property creation: {0} of type {1}.", Name, Value.GetType()));
                    UpsertProperty(Name, Value.GetType(), Value);
                }
            }
            else
                property.SetValue(Value);
        }

        /// <summary>
        /// Same as adding a property, but adding a property twice is not an error and will not erase
        /// the current value.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        public void UpsertProperty(String Name, System.Type Type, Object Value)
        {
            if (!Properties.ContainsKey(Name))
                AddProperty(Name, Type);

            if (Properties[Name].PropertyType != Type)
            {
                Core.LogWarning(String.Format("Upsert property changed type: {0} changed to type {1}.", Name, Type.Name));
                Properties.Upsert(Name, new MudObjectProperty(Type));
            }

            SetProperty(Name, Value);
        }

        public void UpsertProperty<T>(String Name, T Value)
        {
            UpsertProperty(Name, typeof(T), Value);
        }

        public T GetProperty<T>(String Name)
        {
            MudObjectProperty property = null;
            if (!Properties.TryGetValue(Name, out property))
                throw new InvalidOperationException("Mud Object does not have a property named " + Name);
            return property.As<T>();
        }

        public T GetPropertyOrDefault<T>(String Name, T Default)
        {
            MudObjectProperty property = null;
            if (!Properties.TryGetValue(Name, out property))
                return Default;
            return property.As<T>();
        }
        
        public bool HasProperty(String Name)
        {
            return Properties.ContainsKey(Name);
        }

        public bool HasProperty<T>(String Name)
        {
            return Properties.ContainsKey(Name) && Properties[Name].PropertyType == typeof(T);
        }

        #endregion

        private void SetupProperties()
        {
            UpsertProperty("Short", "object");
            UpsertProperty("Long", "");
            UpsertProperty("Article", "a");
            UpsertProperty("Nouns", new NounList());
        }

		public MudObject()
		{
            SetupProperties();

		    State = ObjectState.Alive;
            IsPersistent = false;
		}

        public MudObject(String Short, String Long)
        {
            SetupProperties();
            SetProperty("Short", Short);
            SetProperty("Long", Long);
            GetProperty<NounList>("Nouns").Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                SetProperty("Article", "an");

            State = ObjectState.Alive;
            IsPersistent = false;

        }

        public void SimpleName(String Short, params String[] Synonyms)
        {
            SetProperty("Short", Short);
            GetProperty<NounList>("Nouns").Add(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            GetProperty<NounList>("Nouns").Add(Synonyms);
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
