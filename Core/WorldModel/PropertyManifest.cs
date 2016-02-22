using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class PropertyInfo
    {
        public System.Type Type;
        public Object DefaultValue;
    }

    public static class PropertyManifest
    {
        private static Dictionary<String, PropertyInfo> RegisteredProperties = new Dictionary<string, PropertyInfo>();

        public static void RegisterProperty(String Name, System.Type Type, Object DefaultValue)
        {
            if (RegisteredProperties.ContainsKey(Name))
            {
                var existingProperty = RegisteredProperties[Name];
                if (existingProperty.Type != Type)
                    throw new InvalidOperationException("Property " + Name + " was re-registered with a different type.");
            }
            RegisteredProperties.Upsert(Name, new PropertyInfo { Type = Type, DefaultValue = DefaultValue });
        }

        public static PropertyInfo GetPropertyInformation(String Name)
        {
            if (RegisteredProperties.ContainsKey(Name))
                return RegisteredProperties[Name];
            return null;
        }

        public static bool CheckPropertyType(String Name, Object Value)
        {
            if (Value == null) return true; // Yeah I guess...
            var info = GetPropertyInformation(Name);
            return (info.Type == Value.GetType() ||
                Value.GetType().IsSubclassOf(info.Type));
        }
    }
}
