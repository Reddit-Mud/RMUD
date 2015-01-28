using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public partial class Player : Actor
	{
        [Persist(typeof(DictionaryStringObjectSerializer))]
        public Dictionary<String, Object> Memory { get; set; }

        public Player()
        {
            Memory = new Dictionary<string, object>();
        }

        public void Remember(MudObject For, String Key, Object Value)
        {
            var valueName = For.GetFullName() + ":" + Key;
            Memory.Upsert(valueName, Value);
        }

        public Object Recall(MudObject For, String Key)
        {
            var valueName = For.GetFullName() + ":" + Key;
            Object value = null;
            if (Memory.TryGetValue(valueName, out value))
                return value;
            else
                return null;
        }

        public T Recall<T>(MudObject For, String Key)
        {
            var value = Recall(For, Key);
            if (value != null && value is T) return (T)value;
            return default(T);
        }

        public bool TryRecall<T>(MudObject For, String Key, out T Value) where T: class
        {
            Value = null;
            var valueName = For.GetFullName() + ":" + Key;
            Object value = null;
            if (Memory.TryGetValue(valueName, out value))
            {
                Value = value as T;
                return (value is T);
            }
            else
                return false;            
        }
	}
}
