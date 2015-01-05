using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    public class PersistentValueSerializer
    {
        public static Dictionary<String, PersistentValueSerializer> GlobalSerializers = new Dictionary<String, PersistentValueSerializer>();

        public static void AddGlobalSerializer(PersistentValueSerializer Serializer)
        {
            GlobalSerializers.Upsert(Serializer.TargetType.Name, Serializer);
        }

        public Type TargetType;

        public virtual void WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            throw new NotImplementedException();
        }

        public virtual Object ReadValue(Type ValueType, JsonReader Reader, MudObject Owner)
        {
            throw new NotImplementedException();
        }
    }
}